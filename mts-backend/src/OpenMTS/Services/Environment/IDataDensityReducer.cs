using OpenMTS.Models.Environmnt;
using System.Collections.Generic;

namespace OpenMTS.Services.Environment
{
    // TODO: comments
    public interface IDataDensityReducer
    {
        IEnumerable<DataPoint> ReduceDensity(IEnumerable<DataPoint> data);
    }
}
