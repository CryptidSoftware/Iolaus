using Iolaus.Utils;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Iolaus.Config
{
    public class FileConfigurationProvider : IConfigurationProvider
    {
        private readonly string _fileName;
        private readonly string _text;

        public FileConfigurationProvider(string fileName)
        {
            _fileName = fileName;
            _text = File.ReadAllText(_fileName);
        }

        public Configuration[] GetConfigurations()
        {
            return JArray
                .Parse(_text)
                .Select(token =>
                    Configuration
                        .Parse(token.ToString()))
                .WhereSome()
                .ToArray();
        }

        //TODO: Look into full implementation for FileConfigurationProvider
        public Task<Configuration[]> GetConfigurationsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task AddConfigurationAsync(Configuration configuration)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveConfigurationAsync(string pattern)
        {
            throw new System.NotImplementedException();
        }
    }
}