using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using OpenMTS.Repositories;
using OpenMTS.Services.Environment;
using OpenMTS.Services.Support;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service for managing storage site environmental data.
    /// </summary>
    public class EnvironmentService : ISubscriber<StorageSite>
    {
        /// <summary>
        /// A factory to create loggers from.
        /// </summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Provides storage sites to manage environment data for.
        /// </summary>
        private LocationsService LocationsService { get; }

        /// <summary>
        /// Repository of environmental data.
        /// </summary>
        private IEnvironmentalDataRepository EnvironmentalDataRepository { get; }

        /// <summary>
        /// Data readers for each known storage site.
        /// </summary>
        private Dictionary<Guid, EnvironmentalDataReader> DataReaders { get; }

        /// <summary>
        /// A queue of environemtn snapshots to write to persist.
        /// </summary>
        private ConcurrentQueue<EnvironmentSnapshot> SnapshotQueue { get; }

        /// <summary>
        /// Data density reducing strategy to use.
        /// </summary>
        IDataDensityReducer DataDensityReducer { get; }

        /// <summary>
        /// The task that consumes environmental snapshots from the queue to persist them.
        /// </summary>
        private Task QueueConsumer { get; }

        /// <summary>
        /// The Kafka endpoint.
        /// </summary>
        private string KafkaEndpoint { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Sets up the service with all necessare components. Gets all known storage sites and starts data readers for each of them.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="configuration">The app configuration.</param>
        /// <param name="locationsService">Provides storage sites.</param>
        /// <param name="environmentalDataRepository">Repository of environmental data.</param>
        /// <param name="dataDensityReducer">Data density reducing strategy.</param>
        public EnvironmentService(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            LocationsService locationsService,
            IEnvironmentalDataRepository environmentalDataRepository,
            IDataDensityReducer dataDensityReducer)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger<EnvironmentService>();
            LocationsService = locationsService;
            EnvironmentalDataRepository = environmentalDataRepository;
            SnapshotQueue = new ConcurrentQueue<EnvironmentSnapshot>();
            DataDensityReducer = dataDensityReducer;

            // Get Kafka endpoint
            KafkaEndpoint = configuration.GetValue("Kafka", "");
            if (string.IsNullOrEmpty(KafkaEndpoint))
            {
                throw new Exception("Invalid Kafka endpoint provided!");
            }
            else
            {
                Logger.LogInformation($"Configured Kafka endpoint `{KafkaEndpoint}`.");
            }

            // Start reading from snapshot queue
            QueueConsumer = new TaskFactory().StartNew(() =>
            {
                while (true)
                {
                    if (SnapshotQueue.TryDequeue(out EnvironmentSnapshot snapshot))
                    {
                        EnvironmentalDataRepository.RecordEnvironmentalSnapshot(snapshot);
                    }
                    else
                    {
                        Task.Delay(1000);
                    }
                }
            });

            // Set up data readers for all existing storage sites
            DataReaders = new Dictionary<Guid, EnvironmentalDataReader>();
            foreach (StorageSite site in LocationsService.GetAllStorageSites())
            {
                CreateAndRegisterDataReader(site);
            }

            // Subscribe to the creation of new storage sites
            LocationsService.Subscribe(this);

            // TODO: check for failed readers (Kafka not available?) and restart them
        }


        /// <summary>
        /// Gets the latest value for a specific storage site and environmental factor.
        /// </summary>
        /// <param name="siteId">The ID of the storage site to get the latest value for.</param>
        /// <param name="factor">The factor to get the latest value for.</param>
        /// <returns>Returns the latest value, or null.</returns>
        public DataPoint GetLatestValue(Guid siteId, EnvironmentalFactor factor)
        {
            StorageSite site = LocationsService.GetStorageSite(siteId);
            return EnvironmentalDataRepository.GetLatestValue(site, factor);
        }

        /// <summary>
        /// Gets the history of a specific storage site and environmental factor.
        /// </summary>
        /// <param name="siteId">The ID of the storage site to get the history for.</param>
        /// <param name="factor">The factor to get the history for.</param>
        /// <param name="startTime">Start time of the period to get value for.</param>
        /// <param name="endTime">End time of the period to get value for.</param>
        /// <returns>Returns the matching values.</returns>
        public IEnumerable<DataPoint> GetHistory(Guid siteId, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            StorageSite site = LocationsService.GetStorageSite(siteId);
            IEnumerable<DataPoint> history = EnvironmentalDataRepository.GetHistory(site, factor, startTime, endTime);
            if (history.Count() > 0)
            {
                history = DataDensityReducer.ReduceDensity(history);
            }
            return history;
        }

        /// <summary>
        /// Gets the min and max values for a specific storage site and environmental factor.
        /// </summary>
        /// <param name="siteId">The ID of the storage site to get the history for.</param>
        /// <param name="factor">The factor to get the history for.</param>
        /// <param name="startTime">Start time of the period to get value for.</param>
        /// <param name="endTime">End time of the period to get value for.</param>
        /// <returns>Returns the extrema.</returns>
        public Extrema GetExtrema(Guid siteId, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            StorageSite site = LocationsService.GetStorageSite(siteId);
            return EnvironmentalDataRepository.GetExtrema(site, factor, startTime, endTime);
        }

        /// <summary>
        /// Fired when a new storage site has been created - creates a new environment data reader.
        /// </summary>
        /// <param name="site">New storage site.</param>
        public void OnPublish(StorageSite site)
        {
            Logger.LogInformation("New site created - starting an environment data reader...");
            CreateAndRegisterDataReader(site);
        }

        #region Private helpers

        /// <summary>
        /// Creates a new environment data reader, which immediately starts to read data from Kafka.
        /// </summary>
        /// <param name="site">The storage site the new reader is for.</param>
        private void CreateAndRegisterDataReader(StorageSite site)
        {
            DataReaders.Add(site.Id, new EnvironmentalDataReader(LoggerFactory.CreateLogger<EnvironmentalDataReader>(), SnapshotQueue, KafkaEndpoint, site));
        }

        #endregion
    }
}
