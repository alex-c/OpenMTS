using Microsoft.Extensions.Logging;
using OpenMTS.Models;
using OpenMTS.Repositories;
using OpenMTS.Services.Exceptions;
using System.Collections.Generic;

namespace OpenMTS.Services
{
    /// <summary>
    /// A service exposing functionality relating to plastics.
    /// </summary>
    public class PlasticsService
    {
        /// <summary>
        /// The underlying repository of plastics.
        /// </summary>
        private IPlasticsRepository PlasticsRepository { get; }

        /// <summary>
        /// A logger for local logging needs.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlasticsService"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="plasticsRepository">A plastics repository.</param>
        public PlasticsService(ILoggerFactory loggerFactory, IPlasticsRepository plasticsRepository)
        {
            Logger = loggerFactory.CreateLogger<PlasticsService>();
            PlasticsRepository = plasticsRepository;
        }

        /// <summary>
        /// Gets all available plastics.
        /// </summary>
        /// <returns>Returns all plastics.</returns>
        public IEnumerable<Plastic> GetPlastics()
        {
            return PlasticsRepository.GetAllPlastics();
        }

        /// <summary>
        /// Gets and filters plastics using a partial name.
        /// </summary>
        /// <param name="partialName">String to filter with..</param>
        /// <returns>Returns filtered plastics.</returns>
        public IEnumerable<Plastic> GetPlastics(string partialName)
        {
            return PlasticsRepository.SearchPlasticsByName(partialName);
        }

        /// <summary>
        /// Gets a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to get.</param>
        /// <returns>Returns the plastic or null.</returns>
        /// <exception cref="PlasticNotFoundException">Thrown if no matching plastic could not be found.</exception>
        public Plastic GetPlastic(string id)
        {
            return GetPlasticOrThrowNotFoundException(id);
        }

        /// <summary>
        /// Creates a new plastic.
        /// </summary>
        /// <param name="id">The ID of the new plastic.</param>
        /// <param name="name">The name of the plastic to create.</param>
        /// <returns>Returns the newly created plastic.</returns>
        public Plastic CreatePlastic(string id, string name)
        {
            if (IsPlasticsIdTaken(id))
            {
                throw new PlasticAlreadyExistsException(id);
            }
            return PlasticsRepository.CreatePlastic(id, name);
        }

        /// <summary>
        /// Updates a plastic.
        /// </summary>
        /// <param name="id">The ID of the plastic to update.</param>
        /// <param name="name">The name of the plastic to update.</param>
        /// <returns>Returns the updated plastic.</returns>
        /// <exception cref="PlasticNotFoundException">Thrown if no matching material plastic could not be found.</exception>
        public Plastic UpdatePlastic(string id, string name)
        {
            Plastic plastic = GetPlasticOrThrowNotFoundException(id);
            plastic.Name = name;
            PlasticsRepository.UpdatePlastic(plastic);
            return plastic;
        }

        #region Private helpers

        /// <summary>
        /// Determines whether a plastics ID is taken.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>Returns whether the ID is taken.</returns>
        private bool IsPlasticsIdTaken(string id)
        {
            Plastic plastic = PlasticsRepository.GetPlastic(id);
            return plastic != null;
        }

        /// <summary>
        /// Attempts to get a plastic from the underlying repository and throws a <see cref="PlasticNotFoundException"/> if no matching plastic could be found.
        /// </summary>
        /// <param name="id">ID of the plastic to get.</param>
        /// <exception cref="PlasticNotFoundException">Thrown if no matching plastic could be found.</exception>
        /// <returns>Returns the plastic, if found.</returns>
        private Plastic GetPlasticOrThrowNotFoundException(string id)
        {
            Plastic plastic = PlasticsRepository.GetPlastic(id);
            if (plastic == null)
            {
                throw new PlasticNotFoundException($"Could not find any plastic with Id `${id}`.");
            }
            return plastic;
        }

        #endregion
    }
}
