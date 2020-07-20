using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.IO;

namespace OpenMTS
{
    /// <summary>
    /// OpenMTS main prorgam.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // Set up Serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}][{Level:u3}][{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Build and run web host
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Configures and provides the web host builder.
        /// </summary>
        /// <param name="args">Command line arguments to pass through.</param>
        /// <returns>Returns the configured web host builder.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(lb => lb.AddSerilog())
                .UseSerilog();
    }
}
