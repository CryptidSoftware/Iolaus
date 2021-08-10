using System.Threading.Tasks;

namespace Iolaus.Config
{
    public interface IConfigurationProvider
    {
        public Configuration[] GetConfigurations();

        public Task<Configuration[]> GetConfigurationsAsync();

        public Task AddConfigurationAsync(Configuration configuration);

        public Task RemoveConfigurationAsync(string pattern);
    }
}