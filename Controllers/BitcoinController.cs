using API_NodeMonitor.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_NodeMonitor.Controllers;

[ApiController]
[Route("api/bitcoin")]
public class BitcoinController(IBitcoinService bitcoinService) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var data = await bitcoinService.GetAllDataAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Could not fetch data from Bitcoin node.", details = ex.Message });
        }
    }

    [HttpGet("mempool")]
    public async Task<IActionResult> GetMempoolInfo()
    {
        var data = await bitcoinService.GetMempoolInfoAsync();
        return Ok(data);
    }

    [HttpGet("network")]
    public async Task<IActionResult> GetNetworkInfo()
    {
        var data = await bitcoinService.GetNetworkInfoAsync();
        return Ok(data);
    }
    
    [HttpGet("uptime")]
    public async Task<IActionResult> GetUptime()
    {
        var data = await bitcoinService.GetUptimeAsync();
        return Ok(new { uptimeInSeconds = data });
    }
}