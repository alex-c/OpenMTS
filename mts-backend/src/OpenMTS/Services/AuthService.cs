using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenMTS.Models;
using OpenMTS.Services.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service that provides authentication functionality.
    /// </summary>
    public class AuthService
    {
        /// <summary>
        /// Holds known authentication providers for different authentication methods.
        /// </summary>
        private Dictionary<AuthenticationMethod, IAuthenticationProvider> Providers { get; }

        /// <summary>
        /// Signing credentials for JWTs.
        /// </summary>
        private SigningCredentials SigningCredentials { get; }

        /// <summary>
        /// Lifetime of issued JWTs.
        /// </summary>
        private TimeSpan JwtLifetime { get; }

        /// <summary>
        /// Issuer name of issued JWTs.
        /// </summary>
        private string JwtIssuer { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Sets up the service with all known authentication providers.
        /// </summary>
        /// <param name="loggerFactory">Fasctory to create loggers from.</param>
        /// <param name="providers">Known authentication providers.</param>
        /// <param name="configuration">App configuration for JWT signing information.</param>
        public AuthService(ILoggerFactory loggerFactory, IEnumerable<IAuthenticationProvider> providers, IConfiguration configuration)
        {
            Providers = new Dictionary<AuthenticationMethod, IAuthenticationProvider>();
            Logger = loggerFactory.CreateLogger<AuthService>();

            // Register providers
            foreach (IAuthenticationProvider provider in providers)
            {
                if (!Providers.TryAdd(provider.AuthenticationMethod, provider))
                {
                    Logger.LogWarning($"Failed registering an authentication provider for authentication method '{provider.AuthenticationMethod.ToString()}'.");
                }
            }

            // JWT-related configuration
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret")));
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtLifetime = TimeSpan.FromMinutes(configuration.GetValue<int>("Jwt:LifetimeInMinutes"));
            JwtIssuer = configuration.GetValue<string>("Jwt:Issuer");
        }

        /// <summary>
        /// Authenticate to authenticate a client with a given method.
        /// </summary>
        /// <param name="method">Authentication method to use.</param>
        /// <param name="data">Data provided by the client for authentication.</param>
        /// <returns>Returns whether authentication was successful.</returns>
        public bool TryAuthenticate(AuthenticationMethod method, string data, out string serializedToken)
        {
            serializedToken = null;
            if (Providers.TryGetValue(method, out IAuthenticationProvider provider))
            {
                if (provider.TryAuthenticate(data, out string subject, out IEnumerable<Role> roles, out IEnumerable<Right> rights))
                {
                    List<Claim> claims = new List<Claim>
                    {
                        // Add subject
                        new Claim(JwtRegisteredClaimNames.Sub, subject)
                    };

                    // Add role claims
                    foreach (Role role in roles)
                    {
                        claims.Add(new Claim("role", ((int)role).ToString()));
                    }

                    // Add right claims
                    foreach (Right right in rights)
                    {
                        claims.Add(new Claim("right", right.ToString()));
                    }

                    // Generate token
                    JwtSecurityToken token = new JwtSecurityToken(JwtIssuer, null, claims, expires: DateTime.Now.Add(JwtLifetime), signingCredentials: SigningCredentials);
                    serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return true;
                }
                return false;
            }
            else
            {
                throw new Exception($"Missing authentication provider for authentication method '{method.ToString()}'.");
            }
        }
    }
}
