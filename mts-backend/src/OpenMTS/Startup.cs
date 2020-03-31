﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenMTS.Authorization;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Repositories.Memory;
using OpenMTS.Repositories.Mocking;
using OpenMTS.Services;
using OpenMTS.Services.Authentication;
using OpenMTS.Services.Authentication.Providers;
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
        /// CORS policy name for local development.
        /// </summary>
        private readonly string LOCAL_DEVELOPMENT_CORS_POLICY = "localDevelopmentCorsPolicy";

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
        private ILogger Logger { get; }

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
            services.AddCors(options =>
            {
                if (Environment.IsDevelopment())
                {
                    options.AddPolicy(LOCAL_DEVELOPMENT_CORS_POLICY, builder =>
                    {
                        builder.WithOrigins("http://localhost:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          // Header needed on client-side to access a file download file name
                          .WithExposedHeaders("Content-Disposition");
                    });
                }
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
            services.AddAuthorization(options =>
            {
                // Materials
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_MATERIAL, Role.Administrator, RightIds.MATERIALS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_MATERIAL, Role.Administrator, RightIds.MATERIALS_UPDATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_SET_CUSTOM_MATERIAL_PROP_VALUE, Role.Administrator, RightIds.MATERIAL_CUSTOM_PROPS_SET);
                RegisterPolicy(options, AuthPolicyNames.MAY_DELETE_CUSTOM_MATERIAL_PROP_VALUE, Role.Administrator, RightIds.MATERIAL_CUSTOM_PROPS_DELETE);

                // Plastics
                RegisterPolicy(options, AuthPolicyNames.MAY_CREATE_PLASTIC, Role.Administrator, RightIds.PLASTICS_CREATE);
                RegisterPolicy(options, AuthPolicyNames.MAY_UPDATE_PLASTIC, Role.Administrator, RightIds.PLASTICS_UPDATE);

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
                services.AddSingleton<IReadOnlyApiKeyProvider, MockApiKeyRepository>();
                services.AddSingleton<ILocationsRepository, MockLocationsRepository>();
                services.AddSingleton<ICustomBatchPropRepository, MockCustomBatchPropRepository>();
                MockCustomMaterialPropValueRepository mockCustomMaterialPropValueRepository = new MockCustomMaterialPropValueRepository();
                services.AddSingleton<ICustomMaterialPropValueRepository>(mockCustomMaterialPropValueRepository);
                services.AddSingleton(mockCustomMaterialPropValueRepository);
            }
            else
            {
                // TODO: implement repositories for PostgreSQL persistence
                throw new NotImplementedException("PostgreSQL-based persistence hasn't been implemented yet.");
            }
            services.AddSingleton<IRightsRepository>(new MemoryRightsRepository());

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

            // Configure MVC
            services.AddMvc();
        }

        /// <summary>
        /// Configures the app.
        /// </summary>
        /// <param name="app">Application builder to configure the app through.</param>
        public void Configure(IApplicationBuilder app)
        {
            // Use CORS
            if (Environment.IsDevelopment())
            {
                app.UseCors(LOCAL_DEVELOPMENT_CORS_POLICY);
                Logger.LogInformation("Local development CORS policy for 'localhost:8080' enabled.");
            }
            
            // Use JWT-based auth
            app.UseAuthentication();

            // Use MVC
            app.UseMvc();
        }

        private void RegisterPolicy(AuthorizationOptions options, string policyName, Role role, string rightId)
        {
            RegisterPolicy(options, policyName, new Role[] { role }, rightId);
        }

        private void RegisterPolicy(AuthorizationOptions options, string policyName, IEnumerable<Role> roles, string rightId)
        {
            options.AddPolicy(policyName, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(roles, rightId)));
        }
    }
}
