using API_NodeMonitor.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_NodeMonitor.Controllers;

[ApiController]
[Route("api/monero")]
public class MoneroController(IMoneroService moneroService) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var data = await moneroService.GetAllDataAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Could not fetch data from Monero node.", details = ex.Message });
        }
    }
}