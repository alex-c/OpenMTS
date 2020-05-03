using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OpenMTS.Authorization;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.Memory;
using OpenMTS.Repositories.Mocking;
using OpenMTS.Repositories.PostgreSQL;
using OpenMTS.Repositories.PostgreSQL.Support;
using OpenMTS.Services;
using OpenMTS.Services.Authentication;
using OpenMTS.Services.Authentication.Providers;
using OpenMTS.Services.Environment;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMTS
{
    /// <summary>
    /// OpenMTS server startup and configuration.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// CORS policy name for the frontend.
        /// </summary>
        private readonly string FRONTEND_CORS_POLICY = "FrontendCorsPolicy";

        /// <summary>
        /// The hosting environment information.
        /// </summary>
        private IHostingEnvironment Environment { get; }

        /// <summary>
        /// The app configuration.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Logger factory to use locally and pass through to services.
        /// </summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Local logger instance.
        /// </summary>
        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        /// <summary>
        /// Sets up the Startup class with everything needed for configuration.
        /// </summary>
        /// <param name="loggerFactory">Logger factory to create loggers from.</param>
        /// <param name="environment">Hosting environment information.</param>
        /// <param name="configuration">App configuration as set up by the web host builder.</param>
        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment, IConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            Environment = environment;
            Configuration = configuration;
            Logger = LoggerFactory.CreateLogger<Startup>();
        }

        /// <summary>
        /// Configures the available services for dependency injection.
        /// </summary>
        /// <param name="services">Service collection to register services with.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure CORS
            string frontendUrl = Configuration.GetValue<string>("ExternalFrontendUrl");
            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                Logger.LogWarning("No valid frontend URL provided for CORS. This is okay if the server self-hosts the frontend at `./wwwroot`.");
            }
            else
            {
                Logger.LogInformation($"Frontend CORS policy configured for `{frontendUrl}`.");
            }
            services.AddCors(options =>
            {
                options.AddPolicy(FRONTEND_CORS_POLICY, builder =>
                {
                    builder.WithOrigins(frontendUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        // `Content-Disposition` header needed on client-side to access a file download file name!
                        .WithExposedHeaders("Content-Disposition");
                });
            });

            // Configure JWT-based authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration.GetValue<string>("Jwt:Issuer"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:Secret")))
                    };
                });

            // Configure authorization policies
            IEnumerable<Role> allRoles = new Role[] { Role.Administrator, Role.ScientificAssistant, Role.User };
            services.AddAuthorization(options =>
            {
                // Material batches
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_BATCH, allRoles, RightIds.BATCHES_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_BATCH, allRoles, RightIds.BATCHES_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_BATCH_STATUS, new Role[] { Role.Administrator, Role.ScientificAssistant }, RightIds.BATCHES_UPDATE_STATUS);
                RegisterPolicy(options, AuthPolicyNames.MAY_PERFORM_BATCH_TRANSACTION, allRoles, RightIds.BATCHES_PERFORM_TRANSACTION);

                // Materials
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_MATERIAL, allRoles, RightIds.MATERIALS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_MATERIAL, allRoles, RightIds.MATERIALS_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_SET_CUSTOM_MATERIAL_PROP_VALUE, allRoles, RightIds.MATERIAL_CUSTOM_PROPS_SET);
                RegisterPolicy(options, AuthPolicyNames.MAY_DELETE_CUSTOM_MATERIAL_PROP_VALUE, allRoles, RightIds.MATERIAL_CUSTOM_PROPS_DELETE);

                // Plastics
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_PLASTIC, allRoles, RightIds.PLASTICS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_PLASTIC, allRoles, RightIds.PLASTICS_UPDATE);

                // Configuration administration
                RegisterPolicy(options, AuthPolicyNames.MAY_SET_CONFIGURATION, Role.Administrator, RightIds.CONFIGFURATION_SET);
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_CUSTOM_MATERIAL_PROP, Role.Administrator, RightIds.CUSTOM_MATERIAL_PROPS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_CUSTOM_MATERIAL_PROP, Role.Administrator, RightIds.CUSTOM_MATERIAL_PROPS_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_DELETE_CUSTOM_MATERIAL_PROP, Role.Administrator, RightIds.CUSTOM_MATERIAL_PROPS_DELETE);
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_CUSTOM_BATCH_PROP, Role.Administrator, RightIds.CUSTOM_BATCH_PROPS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_CUSTOM_BATCH_PROP, Role.Administrator, RightIds.CUSTOM_BATCH_PROPS_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_DELETE_CUSTOM_BATCH_PROP, Role.Administrator, RightIds.CUSTOM_BATCH_PROPS_DELETE);

                // User administration
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_USER, Role.Administrator, RightIds.USERS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_USER, Role.Administrator, RightIds.USERS_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_USER_STATUS, Role.Administrator, RightIds.USERS_UPDATE_STATUS);

                // API key administration
                RegisterPolicy(options, AuthPolicyNames.MAY_QUERY_KEYS, Role.Administrator, RightIds.KEYS_QUERY);
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_KEY, Role.Administrator, RightIds.KEYS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_KEY, Role.Administrator, RightIds.KEYS_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_KEY_STATUS, Role.Administrator, RightIds.KEYS_UPDATE_STATUS);
                RegisterPolicy(options, AuthPolicyNames.MAY_DELETE_KEY, Role.Administrator, RightIds.KEYS_DELETE);

                // Locations administration
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_STORAGE_SITE, Role.Administrator, RightIds.STORAGE_SITES_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_STORAGE_SITE, Role.Administrator, RightIds.STORAGE_SITES_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_STORAGE_AREA, Role.Administrator, RightIds.STORAGE_AREAS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_STORAGE_AREA, Role.Administrator, RightIds.STORAGE_AREAS_UPDATE);
            });

            // Add authorization handler
            services.AddSingleton<IAuthorizationHandler, AccessRightsHandler>();

            // Set up repositories
            if (Configuration.GetValue<bool>("Mocking:UseMockDataPersistence"))
            {
                if (Configuration.GetValue<bool>("Mocking:SeedWithMockDataOnStartup"))
                {
                    services.AddSingleton<MockDataProvider>();
                }
                services.AddSingleton<IMaterialBatchRepository, MockMaterialBatchRepository>();
                services.AddSingleton<ITransactionRepository, MockTransactionRepository>();
                services.AddSingleton<IMaterialsRepository, MockMaterialsRepository>();
                services.AddSingleton<IPlasticsRepository, MockPlasticsRepository>();
                services.AddSingleton<IConfigurationRepository, MockConfigurationRepository>();
                services.AddSingleton<ICustomMaterialPropRepository, MockCustomMaterialPropRepository>();
                services.AddSingleton<IUserRepository, MockUserRepository>();
                services.AddSingleton<IReadOnlyUserRepository, MockUserRepository>();
                services.AddSingleton<IApiKeyRepository, MockApiKeyRepository>();
                services.AddSingleton<IReadOnlyApiKeyRepository, MockApiKeyRepository>();
                services.AddSingleton<ILocationsRepository, MockLocationsRepository>();
                services.AddSingleton<ICustomBatchPropRepository, MockCustomBatchPropRepository>();
                MockCustomMaterialPropValueRepository mockCustomMaterialPropValueRepository = new MockCustomMaterialPropValueRepository();
                services.AddSingleton<ICustomMaterialPropValueRepository>(mockCustomMaterialPropValueRepository);
                services.AddSingleton(mockCustomMaterialPropValueRepository);
                services.AddSingleton<IEnvironmentalDataRepository, MockEnvironmentalDataRepository>();
                // TODO: IStatsProvider implementation
            }
            else
            {
                // Configure Dapper to convert `column_name` to property `ColumnName` and handle UTC times right
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                Dapper.SqlMapper.AddTypeHandler(new DateTimeHandler());

                // Register services
                services.AddSingleton<IMaterialBatchRepository, PostgreSqlMaterialBatchRepository>();
                services.AddSingleton<ITransactionRepository, PostgreSqlTransactionRepository>();
                services.AddSingleton<IMaterialsRepository, PostgreSqlMaterialsRepository>();
                services.AddSingleton<IPlasticsRepository, PostgreSqlPlasticsRepository>();
                services.AddSingleton<IConfigurationRepository, PostgreSqlConfigurationRepository>();
                services.AddSingleton<ICustomMaterialPropRepository, PostgreSqlCustomMaterialPropRepository>();
                services.AddSingleton<IUserRepository, PostgreSqlUserRepository>();
                services.AddSingleton<IReadOnlyUserRepository, PostgreSqlUserRepository>();
                services.AddSingleton<IApiKeyRepository, PostgreSqlApiKeyRepository>();
                services.AddSingleton<IReadOnlyApiKeyRepository, PostgreSqlApiKeyRepository>();
                services.AddSingleton<ILocationsRepository, PostgreSqlLocationsRepository>();
                services.AddSingleton<ICustomBatchPropRepository, PostgreSqlCustomBatchPropRepository>();
                services.AddSingleton<ICustomMaterialPropValueRepository, PostgreSqlCustomMaterialPropValueRepository>();
                services.AddSingleton<IEnvironmentalDataRepository, PostgreSqlEnvironmentalDataRepository>();
                services.AddSingleton<IStatsProvider, PostgreSqlStatsProvider>();
            }
            services.AddSingleton<IRightsRepository, MemoryRightsRepository>();

            // Data density reducing strategy
            services.AddTransient<IDataDensityReducer, RamerDouglasPeuckerDataDensityReducer>();

            // Check JWT signing key validity
            if (Configuration.GetValue<string>("Jwt:Secret").Length < 16)
            {
                string errorMessage = "The secret for signing JWTs has to be at least 16 characters long.";
                Logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            // Register auth providers
            services.AddSingleton<IAuthenticationProvider, UserLoginAuthenticationProvider>();
            services.AddSingleton<IAuthenticationProvider, GuestLoginAuthenticationProvider>();
            services.AddSingleton<IAuthenticationProvider, ApiKeyAuthenticationProvider>();

            // Register services
            services.AddSingleton<AuthService>();
            services.AddSingleton<PasswordHashingService>();
            services.AddSingleton<InventoryService>();
            services.AddSingleton<TransactionLogService>();
            services.AddSingleton<MaterialsService>();
            services.AddSingleton<PlasticsService>();
            services.AddSingleton<ConfigurationService>();
            services.AddSingleton<CustomMaterialPropService>();
            services.AddSingleton<CustomBatchPropService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<ApiKeyService>();
            services.AddSingleton<RightsService>();
            services.AddSingleton<LocationsService>();
            services.AddSingleton<EnvironmentService>();
            services.AddSingleton<StatsService>();
            services.AddSingleton<TraceService>();

            // Optionally ensure that there is an admin account
            IConfiguration ensureAdmin = Configuration.GetSection("EnsureAdmin");
            if (ensureAdmin.GetValue<string>("id") != "")
            {
                string id = ensureAdmin.GetValue<string>("id");
                string name = ensureAdmin.GetValue<string>("name");
                string password = ensureAdmin.GetValue<string>("password");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                {
                    throw new Exception("The `EnsureAdmin` configuration section is invalid: either leave the ID field empty, or fill in all fields.");
                }
                IUserRepository users = services.BuildServiceProvider().GetService<IUserRepository>();
                if (users.GetUser(id) == null)
                {
                    (string hashedPassword, byte[] salt) = new PasswordHashingService(LoggerFactory).HashAndSaltPassword(password);
                    users.CreateUser(id, name, hashedPassword, salt, Role.Administrator);
                }
                Logger.LogInformation($"Ensured existence of administrator account `id`.");
            }

            // Configure MVC
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
        }

        /// <summary>
        /// Configures the app.
        /// </summary>
        /// <param name="app">Application builder to configure the app through.</param>
        public void Configure(IApplicationBuilder app)
        {
            // CORS policy for the frontend
            app.UseCors(FRONTEND_CORS_POLICY);

            // Static files
            string frontendUrl = Configuration.GetValue<string>("ExternalFrontendUrl");
            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                app.UseDefaultFiles();
                app.UseStaticFiles();
            }

            // Use JWT-based auth
            app.UseAuthentication();

            // Use MVC
            app.UseMvc();
        }

        /// <summary>
        /// Registers an authorization policy.
        /// </summary>
        /// <param name="options">The authorization options to extend.</param>
        /// <param name="policyName">Name of the policy tor egister.</param>
        /// <param name="role">The role that grants authorization.</param>
        /// <param name="rightId">The right that grants authorization.</param>
        private void RegisterPolicy(AuthorizationOptions options, string policyName, Role role, string rightId)
        {
            RegisterPolicy(options, policyName, new Role[] { role }, rightId);
        }

        /// <summary>
        /// Registers an authorization policy.
        /// </summary>
        /// <param name="options">The authorization options to extend.</param>
        /// <param name="policyName">Name of the policy tor egister.</param>
        /// <param name="roles">The roles that grant authorization.</param>
        /// <param name="rightId">The right that grants authorization.</param>
        private void RegisterPolicy(AuthorizationOptions options, string policyName, IEnumerable<Role> roles, string rightId)
        {
            options.AddPolicy(policyName, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(roles, rightId)));
        }
    }
}
