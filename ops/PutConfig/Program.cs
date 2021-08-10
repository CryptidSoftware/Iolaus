using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using dotnet_etcd;

namespace PutConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            var etcd = new EtcdClient("http://localhost:2379");
            var configText = File.ReadAllText(args[0]);
            var array = JArray.Parse(configText);

            foreach(var token in array)
            {
                var config = token as JObject;
                var pattern = config["Pattern"].ToString();
                etcd.Put($"iolaus-{pattern}",config.ToString(Formatting.None));
            }

        }
    }
}
