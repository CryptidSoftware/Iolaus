using dotnet_etcd;
using Etcdserverpb;
using Iolaus.Utils;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Iolaus.Config.Etcd
{
    public class EtcdConfigProvider : IConfigurationProvider
    {
        private EtcdClient _client;
        private readonly string _etcdPrefix = "iolaus-";
        //public event EventHandler<ConfigurationUpdate> ConfigUpdated;

        public EtcdConfigProvider(EtcdClient client)
        {
            _client = client;
            WatchRequest request = EtcdUtils.PrefixWatchRequest(_etcdPrefix);
            //_client.Watch(request, OnConfigUpdated);
        }

        public Configuration[] GetConfigurations()
        {
            return _client
                .GetRangeVal(_etcdPrefix)
                .Select(p =>
                    Configuration.Parse(p.Value))
                .WhereSome()
                .ToArray();
        }

        public async Task<Configuration[]> GetConfigurationsAsync()
        {
            return (await _client
                .GetRangeValAsync(_etcdPrefix))
                .Select(p =>
                    Configuration.Parse(p.Value))
                .WhereSome()
                .ToArray();
        }

        public async Task AddConfigurationAsync(Configuration configuration)
        {
            var pattern = configuration.Pattern.PatternAsString;
            var isPatternStored = (await _client.GetAsync($"{_etcdPrefix}{pattern}")).Count > 0;
            
            if (isPatternStored)
            {
                throw new Exception($"Pattern {pattern} already exists");
            }
            
            await _client.PutAsync($"iolaus-{pattern}", configuration.ToString());
        }

        public async Task RemoveConfigurationAsync(string pattern)
        {
            await _client.DeleteAsync($"{_etcdPrefix}{pattern}");
        }
    }
}