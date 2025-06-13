using API_NodeMonitor.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace API_NodeMonitor.Services;

public class BitcoinService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IBitcoinService
{
    private readonly string _bitcoinUrl = configuration["NodeEndpoints:Bitcoin:Url"] ?? string.Empty;
    private readonly string _authHeader = GetAuthHeader(configuration);

    private static string GetAuthHeader(IConfiguration config)
    {
        var user = config["NodeEndpoints:Bitcoin:User"];
        var password = config["NodeEndpoints:Bitcoin:Password"];
        var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
        return $"Basic {authString}";
    }
    
    private async Task<JsonNode> MakeRpcRequestAsync(string method, params object[] @params)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(_authHeader);

        var requestBody = new
        {
            jsonrpc = "1.0",
            id = "dotnet-api-call",
            method = method,
            @params = @params ?? []
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(_bitcoinUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"RPC call to {method} failed: {response.StatusCode} | {error}");
        }

        var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        if (jsonNode?["error"] != null)
        {
            throw new Exception($"RPC Error: {jsonNode["error"]}");
        }
        return jsonNode?["result"] ?? new JsonObject();
    }

    public async Task<long> GetUptimeAsync()
    {
        var result = await MakeRpcRequestAsync("uptime");
        return result.GetValue<long>();
    }

    public async Task<NetworkInfoDto> GetNetworkInfoAsync()
    {
        var result = await MakeRpcRequestAsync("getnetworkinfo");
        return new NetworkInfoDto
        {
            Version = result?["version"]?.GetValue<int>() ?? 0,
            SubVersion = result?["subversion"]?.GetValue<string>() ?? string.Empty,
            ProtocolVersion = result?["protocolversion"]?.GetValue<int>() ?? 0,
            ConnectionsIn = result?["connections_in"]?.GetValue<int>() ?? 0,
            ConnectionsOut = result?["connections_out"]?.GetValue<int>() ?? 0
        };
    }

    public async Task<MempoolInfoDto> GetMempoolInfoAsync()
    {
        var result = await MakeRpcRequestAsync("getmempoolinfo");
        return new MempoolInfoDto
        {
            Size = result?["size"]?.GetValue<int>() ?? 0,
            Bytes = result?["bytes"]?.GetValue<long>() ?? 0,
            Usage = result?["usage"]?.GetValue<long>() ?? 0,
            TotalFee = result?["total_fee"]?.GetValue<decimal>() ?? 0
        };
    }

    public async Task<BitcoinStatusDto> GetAllDataAsync()
    {
        var blockchainInfoTask = MakeRpcRequestAsync("getblockchaininfo");
        var networkInfoTask = GetNetworkInfoAsync();
        var mempoolInfoTask = GetMempoolInfoAsync();
        var uptimeTask = GetUptimeAsync();
        
        await Task.WhenAll(blockchainInfoTask, networkInfoTask, mempoolInfoTask, uptimeTask);

        var blockchainInfo = await blockchainInfoTask;
        var networkInfo = await networkInfoTask;
        var mempoolInfo = await mempoolInfoTask;
        var uptime = await uptimeTask;

        return new BitcoinStatusDto
        {
            Chain = blockchainInfo?["chain"]?.GetValue<string>() ?? string.Empty,
            BlockCount = blockchainInfo?["blocks"]?.GetValue<long>() ?? 0,
            HeaderCount = blockchainInfo?["headers"]?.GetValue<long>() ?? 0,
            Difficulty = blockchainInfo?["difficulty"]?.GetValue<double>() ?? 0,
            Mediantime = blockchainInfo?["mediantime"]?.GetValue<long>() ?? 0,
            VerificationProgress = blockchainInfo?["verificationprogress"]?.GetValue<double>() ?? 0,
            IsPruned = blockchainInfo?["pruned"]?.GetValue<bool>() ?? false,
            SizeOnDisk = blockchainInfo?["size_on_disk"]?.GetValue<ulong>() ?? 0,
            NodeVersion = networkInfo.Version.ToString(),
            UserAgent = networkInfo.SubVersion,
            ProtocolVersion = networkInfo.ProtocolVersion,
            ConnectionsIn = networkInfo.ConnectionsIn,
            ConnectionsOut = networkInfo.ConnectionsOut,
            Uptime = uptime,
            Mempool = mempoolInfo
        };
    }
}