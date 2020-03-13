using System;

namespace Test.Common.Constants
{
    /// <summary>
    /// Constant values used by orleans.
    /// </summary>
    public class HostConstants
    {
        //   public const string PUBSUB_PROVIDER = "PubSubStore";
        //    public const string STORAGE_PROVIDER = "ORLEANS_SIGNALR_STORAGE_PROVIDER";
        /// <summary>
        /// used by orleans streams/notifications.
        /// </summary>
        public const string STREAM_NAMESPACE = "SERVERS_STREAM";
        /// <summary>
        /// Stream provider, SIGNALR.
        /// </summary>
        public const string STREAM_PROVIDER = "ORLEANS_SIGNALR_STREAM_PROVIDER";
        //     public static readonly Guid CLIENT_DISCONNECT_STREAM_ID = Guid.Parse("bdcff7e7-3734-48ab-8599-17d915011b85");
        /// <summary>
        /// The Master all broadcast channel used for all nodes.
        /// </summary>
        /// <returns></returns>
        public static readonly Guid ALL_STREAM_ID = Guid.Parse("fbe53ecd-d896-4916-8281-5571d6733566");
    }
}