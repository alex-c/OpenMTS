﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenMTS.Controllers.Contracts.Responses;
using OpenMTS.Models;
using OpenMTS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Controllers
{
    /// <summary>
    /// A route granting access to materials data and administration.
    /// </summary>
    /// <seealso cref="OpenMTS.Controllers.ControllerBase" />
    [Route("api/materials")]
    public class MaterialsController : ControllerBase
    {
        /// <summary>
        /// The underlying service for materials functionality.
        /// </summary>
        private MaterialsService MaterialsService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsController"/> class.
        /// </summary>
        /// <param name="loggerFactory">A factory to create loggers from.</param>
        /// <param name="materialsService">The materials service.</param>
        public MaterialsController(ILoggerFactory loggerFactory, MaterialsService materialsService)
        {
            Logger = loggerFactory.CreateLogger<MaterialsController>();
            MaterialsService = materialsService;
        }

        /// <summary>
        /// Gets, filters and paginates materials.
        /// </summary>
        /// <param name="page">The page to display.</param>
        /// <param name="elementsPerPage">The number of elements to display per page.</param>
        /// <param name="search">A string to search in materials names.</param>
        /// <param name="manufacturer">The manufacturer to filter with.</param>
        /// <param name="type">Type of the material to filter with.</param>
        /// <returns>Returns a filtered, paginated list of materials.</returns>
        [HttpGet]
        public IActionResult GetMaterials(
            [FromQuery] int page = 1,
            [FromQuery] int elementsPerPage = 10,
            [FromQuery] string search = null,
            [FromQuery] string manufacturer = null,
            [FromQuery] int? type = null)
        {
            // Attempt to parse type, if submitted
            MaterialType? materialType = null;
            if (type != null)
            {
                if (Enum.IsDefined(typeof(MaterialType), type))
                {
                    materialType = (MaterialType)type;
                }
                else
                {
                    return HandleBadRequest("An invalid material type was submitted.");
                }
            }

            // Get, filter and paginate materials
            try
            {
                IEnumerable<Material> materials = MaterialsService.GetMaterials(search, manufacturer, materialType);
                IEnumerable<Material> paginatedMaterials = materials.Skip((page - 1) * elementsPerPage).Take(elementsPerPage);
                return Ok(new PaginatedResponse(paginatedMaterials, materials.Count()));
            }
            catch (Exception exception)
            {
                return HandleUnexpectedException(exception);
            }
        }
    }
}