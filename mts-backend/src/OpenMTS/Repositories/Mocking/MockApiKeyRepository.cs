using OpenMTS.Models;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockApiKeyRepository : IApiKeyRepository, IReadOnlyApiKeyProvider
    {
        private Dictionary<Guid, ApiKey> ApiKeys { get; }

        public MockApiKeyRepository(MockDataProvider mockDataProvider = null)
        {
            if (mockDataProvider == null)
            {
                ApiKeys = new Dictionary<Guid, ApiKey>();
            }
            else
            {
                ApiKeys = mockDataProvider.ApiKeys;
            }
        }

        public IEnumerable<ApiKey> GetAllApiKeys()
        {
            return ApiKeys.Values;
        }

        public ApiKey GetApiKey(Guid id)
        {
            return ApiKeys.GetValueOrDefault(id);
        }

        public ApiKey CreateApiKey(string name)
        {
            ApiKey key = new ApiKey()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Enabled = false,
                Rights = new List<Right>()
            };

            ApiKeys.Add(key.Id, key);

            return key;
        }

        public void UpdateApiKey(ApiKey apiKey)
        {
            ApiKeys[apiKey.Id] = apiKey;
        }

        public void DeleteApiKey(Guid id)
        {
            ApiKeys.Remove(id);
        }
    }
}
