using OpenMTS.Models;

namespace OpenMTS.Repositories.Mocking
{
    public class MockConfigurationRepository : IConfigurationRepository
    {
        private Configuration Configuration { get; set; }

        public MockConfigurationRepository()
        {
            Configuration = new Configuration()
            {
                AllowGuestLogin = false
            };
        }

        public Configuration GetConfiguration()
        {
            return Configuration;
        }

        public void SetConfiguration(Configuration configuration)
        {
            Configuration = configuration;
        }
    }
}
