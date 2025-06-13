using API_NodeMonitor.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace API_NodeMonitor.Services;

public class MoneroService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IMoneroService
{
    private readonly string _moneroUrl = configuration["NodeEndpoints:Monero:Url"] ?? string.Empty;
    
    private async Task<JsonNode> MakeRpcRequestAsync(string method, object? @params = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var requestBody = new
        {
            jsonrpc = "2.0",
            id = "0",
            method = method,
            @params = @params ?? new { }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(_moneroUrl, content);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonNode = JsonNode.Parse(responseString);
        return jsonNode?["result"] ?? new JsonObject();
    }
    
    public async Task<MoneroStatusDto> GetAllDataAsync()
    {
        var info = (await MakeRpcRequestAsync("get_info")) as JsonObject;
        var blockCount = info?["height"]?.GetValue<long>() ?? 0;
        var targetHeight = info?["target_height"]?.GetValue<long>() ?? 0;

        return new MoneroStatusDto
        {
            Status = info?["status"]?.GetValue<string>() ?? "Unknown",
            BlockCount = blockCount,
            TargetHeight = targetHeight,
            Difficulty = info?["difficulty"]?.GetValue<ulong>() ?? 0,
            CumulativeDifficulty = info?["cumulative_difficulty"]?.GetValue<ulong>() ?? 0,
            IncomingConnections = info?["incoming_connections_count"]?.GetValue<int>() ?? 0,
            OutgoingConnections = info?["outgoing_connections_count"]?.GetValue<int>() ?? 0,
            IsSynced = targetHeight == 0 || targetHeight <= blockCount,
            Version = info?["version"]?.GetValue<string>() ?? string.Empty,
            GreyPeerlistSize = info?["grey_peerlist_size"]?.GetValue<int>() ?? 0,
            WhitePeerlistSize = info?["white_peerlist_size"]?.GetValue<int>() ?? 0
        };
    }
}