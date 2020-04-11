using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Services;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// An endpoint for querying information and statistics which cross service and model boundaries.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/stats"), Authorize]
    public class StatsController : ControllerBase
    {
        /// <summary>
        /// Provides stats-related data.
        /// </summary>
        private StatsService StatsService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatsController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="statsService">The stats service.</param>
        public StatsController(ILoggerFactory loggerFactory, StatsService statsService)
        {
            Logger = loggerFactory.CreateLogger<StatsController>();
            StatsService = statsService;
        }

        /// <summary>
        /// Gets an overview of all available storage sites, which includes latest environmental values and total material.
        /// </summary>
        /// <returns>Returns the overview of each storage.</returns>
        [HttpGet("sites/overview")]
        public IActionResult GetSitesOverview()
        {
            return Ok(StatsService.GetSitesOverview());
        }
    }
}
