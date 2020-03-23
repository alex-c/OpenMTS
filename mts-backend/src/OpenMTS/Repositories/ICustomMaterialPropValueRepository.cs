using System;

namespace OpenMTS.Repositories
{
    /// <summary>
    /// A write-only repository for custom material prop values. Reading of custom material prop values is done through the material repository.
    /// </summary>
    public interface ICustomMaterialPropValueRepository
    {
        /// <summary>
        /// Sets a custom material property of the text type.
        /// </summary>
        /// <param name="materialId">The ID of the material to set the prop value for.</param>
        /// <param name="propId">The ID of the prop to set.</param>
        /// <param name="text">The text value to set.</param>
        void SetCustomTextMaterialProp(int materialId, Guid propId, string text);

        /// <summary>
        /// Deletes a custom material property of the text type.
        /// </summary>
        /// <param name="materialId">The ID of the material to delete the prop value for.</param>
        /// <param name="propId">The ID of the prop to unset.</param>
        void RemoveCustomTextMaterialProp(int materialId, Guid propId);

        // TODO: file format?
        void SetCustomFileMaterialProp(int materialId, Guid propId);

        void RemoveCustomFileMaterialProp(int materialId, Guid propId);
    }
}
