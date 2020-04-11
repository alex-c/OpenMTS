using OpenMTS.Models;
using OpenMTS.Models.Environmnt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockEnvironmentalDataRepository : IEnvironmentalDataRepository
    {
        private Dictionary<Guid, List<DataPoint>> Temperature { get; }
        private Dictionary<Guid, List<DataPoint>> Humidity { get; }

        public MockEnvironmentalDataRepository()
        {
            Temperature = new Dictionary<Guid, List<DataPoint>>();
            Humidity = new Dictionary<Guid, List<DataPoint>>();
        }

        public DataPoint GetLatestValue(StorageSite site, EnvironmentalFactor factor)
        {
            DataPoint dataPoint = null;
            switch (factor)
            {
                case EnvironmentalFactor.Temperature:
                    if (Temperature.TryGetValue(site.Id, out List<DataPoint> temperatures))
                    {
                        dataPoint = temperatures.LastOrDefault();
                    }
                    break;
                case EnvironmentalFactor.Humidity:
                    if (Humidity.TryGetValue(site.Id, out List<DataPoint> humidities))
                    {
                        dataPoint = humidities.LastOrDefault();
                    }
                    break;
                default: throw new NotImplementedException();
            }
            return dataPoint;
        }

        public Extrema GetExtrema(StorageSite site, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            Extrema extrema = null;
            switch (factor)
            {
                case EnvironmentalFactor.Temperature:
                    if (Temperature.TryGetValue(site.Id, out List<DataPoint> temperatures))
                    {
                        if (temperatures.Count > 0)
                        {
                            extrema = new Extrema()
                            {
                                MaxValue = temperatures.Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime).Max(t => t.Value),
                                MinValue = temperatures.Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime).Min(t => t.Value)
                            };
                        }
                    }
                    break;
                case EnvironmentalFactor.Humidity:
                    if (Humidity.TryGetValue(site.Id, out List<DataPoint> humidities))
                    {
                        if (humidities.Count > 0)
                        {
                            extrema = new Extrema()
                            {
                                MaxValue = humidities.Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime).Max(t => t.Value),
                                MinValue = humidities.Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime).Min(t => t.Value)
                            };
                        }
                    }
                    break;
                default: throw new NotImplementedException();
            }
            return extrema;
        }

        public IEnumerable<DataPoint> GetHistory(StorageSite site, EnvironmentalFactor factor, DateTime startTime, DateTime endTime)
        {
            IEnumerable<DataPoint> history = new List<DataPoint>();
            switch (factor)
            {
                case EnvironmentalFactor.Temperature:
                    if (Temperature.TryGetValue(site.Id, out List<DataPoint> temperatures))
                    {
                        history = temperatures.Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime);
                    }
                    break;
                case EnvironmentalFactor.Humidity:
                    if (Humidity.TryGetValue(site.Id, out List<DataPoint> humidities))
                    {
                        history = humidities.Where(t => t.Timestamp >= startTime && t.Timestamp <= endTime);
                    }
                    break;
                default: throw new NotImplementedException();
            }
            return history;
        }

        public void RecordEnvironmentalSnapshot(EnvironmentSnapshot snapshot)
        {
            if (!Temperature.ContainsKey(snapshot.StorageSiteId))
            {
                Temperature.Add(snapshot.StorageSiteId, new List<DataPoint>());
            }
            if (!Humidity.ContainsKey(snapshot.StorageSiteId))
            {
                Humidity.Add(snapshot.StorageSiteId, new List<DataPoint>());
            }
            if (snapshot.Temperature.HasValue)
            {
                Temperature[snapshot.StorageSiteId].Add(new DataPoint()
                {
                    Timestamp = snapshot.Timestamp,
                    Value = snapshot.Temperature.Value
                });
            }
            if (snapshot.Humidity.HasValue)
            {
                Humidity[snapshot.StorageSiteId].Add(new DataPoint()
                {
                    Timestamp = snapshot.Timestamp,
                    Value = snapshot.Humidity.Value
                });
            }
        }
    }
}
