using System;
using System.Collections.Generic;

public class PeerConfiguration
{
    public string WebServices { get; }

    public string ServerGameVersion { get; }

    public string WebServicesAuth { get; }

    public int HeartbeatInterval { get; }

    public int HeartbeatTimeout { get; }

    public IReadOnlyList<byte[]> CompositeHashes { get; }

    public IReadOnlyList<byte[]> JunkHashes { get; }

    public PeerConfiguration(string webServices, string webServicesAuth, string version, int heartbeatInterval, int heartbeatTimeout, IReadOnlyList<byte[]> compositeHashes, IReadOnlyList<byte[]> junkHashes)
    {
        WebServicesAuth = webServicesAuth;
        WebServices = webServices ?? throw new ArgumentNullException("webServices");
        ServerGameVersion = ServerGameVersion;
        CompositeHashes = compositeHashes ?? throw new ArgumentNullException("compositeHashes");
        JunkHashes = junkHashes ?? throw new ArgumentNullException("junkHashes");
        if (heartbeatInterval <= 0)
        {
            throw new ArgumentOutOfRangeException("heartbeatInterval");
        }
        if (heartbeatTimeout <= 0)
        {
            throw new ArgumentOutOfRangeException("heartbeatTimeout");
        }
        HeartbeatInterval = heartbeatInterval;
        HeartbeatTimeout = heartbeatTimeout;
    }
}