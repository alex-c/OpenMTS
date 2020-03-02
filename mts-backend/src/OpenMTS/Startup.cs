using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                          .AllowCredentials();
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
                options.AddPolicy(AuthPolicyNames.MAY_SET_CONFIGURATION, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.CONFIGFURATION_SET)));

                // User administration
                options.AddPolicy(AuthPolicyNames.MAY_CREATE_USER, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.USERS_CREATE)));
                options.AddPolicy(AuthPolicyNames.MAY_UPDATE_USER, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.USERS_UPDATE)));
                options.AddPolicy(AuthPolicyNames.MAY_UPDATE_USER_STATUS, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.USERS_UPDATE_STATUS)));

                // API key administration
                options.AddPolicy(AuthPolicyNames.MAY_QUERY_KEYS, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.KEYS_QUERY)));
                options.AddPolicy(AuthPolicyNames.MAY_CREATE_KEY, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.KEYS_CREATE)));
                options.AddPolicy(AuthPolicyNames.MAY_UPDATE_KEY, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.KEYS_UPDATE)));
                options.AddPolicy(AuthPolicyNames.MAY_UPDATE_KEY_STATUS, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.KEYS_UPDATE_STATUS)));
                options.AddPolicy(AuthPolicyNames.MAY_DELETE_KEY, policy => policy.RequireAuthenticatedUser().Requirements.Add(new AccessRightsRequirement(Role.Administrator, RightIds.KEYS_DELETE)));
            });

            // Add authorization handler
            services.AddSingleton<IAuthorizationHandler, AccessRightsHandler>();

            // Set up repositories
            if (Configuration.GetValue<bool>("Mocking:UseMockDataPersistence"))
            {
                MockDataProvider dataProvider = null;
                if (Configuration.GetValue<bool>("Mocking:SeedWithMockDataOnStartup"))
                {
                    dataProvider = new MockDataProvider(new PasswordHashingService());
                }
                services.AddSingleton<IConfigurationRepository>(new MockConfigurationRepository());
                MockUserRepository userRepository = new MockUserRepository(dataProvider);
                services.AddSingleton<IReadOnlyUserRepository>(userRepository);
                services.AddSingleton<IUserRepository>(userRepository);
                services.AddSingleton<IApiKeyRepository>(new MockApiKeyRepository());
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
            services.AddSingleton<PasswordHashingService>();
            services.AddSingleton<ConfigurationService>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<ApiKeyService>();
            services.AddSingleton<RightsService>();

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
    }
}
