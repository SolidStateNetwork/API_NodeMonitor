using API_NodeMonitor.DTOs;

namespace API_NodeMonitor.Services;

public interface IMoneroService
{
    Task<MoneroStatusDto> GetAllDataAsync();
}