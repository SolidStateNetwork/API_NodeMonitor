namespace API_NodeMonitor.DTOs;

public class BitcoinStatusDto
{
    public string Chain { get; set; } = string.Empty;
    public long BlockCount { get; set; }
    public long HeaderCount { get; set; }
    public double Difficulty { get; set; }
    public long Mediantime { get; set; }
    public double VerificationProgress { get; set; }
    public bool IsPruned { get; set; }
    public ulong SizeOnDisk { get; set; }
    public string NodeVersion { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public int ProtocolVersion { get; set; }
    public int ConnectionsIn { get; set; }
    public int ConnectionsOut { get; set; }
    public long Uptime { get; set; }
    public MempoolInfoDto Mempool { get; set; } = new();
}

public class MempoolInfoDto
{
    public int Size { get; set; }
    public long Bytes { get; set; }
    public long Usage { get; set; }
    public decimal TotalFee { get; set; }
}

public class NetworkInfoDto
{
    public string Version { get; set; } = string.Empty;
    public string SubVersion { get; set; } = string.Empty;
    public int ProtocolVersion { get; set; }
    public int ConnectionsIn { get; set; }
    public int ConnectionsOut { get; set; }
}