namespace API_NodeMonitor.DTOs;

public class MoneroStatusDto
{
    public string Status { get; set; } = string.Empty;
    public long BlockCount { get; set; }
    public long TargetHeight { get; set; }
    public ulong Difficulty { get; set; }
    public ulong CumulativeDifficulty { get; set; }
    public int IncomingConnections { get; set; }
    public int OutgoingConnections { get; set; }
    public bool IsSynced { get; set; }
    public string Version { get; set; } = string.Empty;
    public int GreyPeerlistSize { get; set; }
    public int WhitePeerlistSize { get; set; }
}