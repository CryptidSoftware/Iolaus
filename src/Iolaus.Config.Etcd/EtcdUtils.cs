using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;

namespace Iolaus.Config.Etcd
{
    public class EtcdUtils
    {
        public static WatchRequest PrefixWatchRequest(string prefix)
        {
            return new WatchRequest
            {
                CreateRequest = new WatchCreateRequest
                {
                    Key = ByteString.CopyFromUtf8(prefix),
                    RangeEnd = ByteString.CopyFromUtf8(EtcdClient.GetRangeEnd(prefix))
                }
            };
        }
    }
}