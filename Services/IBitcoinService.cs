using API_NodeMonitor.DTOs;

namespace API_NodeMonitor.Services;

public interface IBitcoinService
{
    Task<BitcoinStatusDto> GetAllDataAsync();
    Task<MempoolInfoDto> GetMempoolInfoAsync();
    Task<NetworkInfoDto> GetNetworkInfoAsync();
    Task<long> GetUptimeAsync();
}