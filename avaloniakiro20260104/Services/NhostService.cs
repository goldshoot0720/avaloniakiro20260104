using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace avaloniakiro20260104.Services;

public static class NhostService
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<bool> TestConnectionAsync(string subdomain, string adminSecret, string region = "eu-central-1")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(subdomain) || string.IsNullOrWhiteSpace(adminSecret))
            {
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(region))
                region = "eu-central-1";

            var url = $"https://{subdomain}.hasura.{region}.nhost.run/v1/graphql";
            
            // 簡單的健康檢查請求
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-hasura-admin-secret", adminSecret);
            request.Content = new StringContent(
                "{\"query\":\"query { __typename }\"}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"nhost 連接測試失敗: {ex.Message}");
            return false;
        }
    }

    public static string GetGraphQLEndpoint(string subdomain, string region = "eu-central-1")
    {
        if (string.IsNullOrWhiteSpace(region))
            region = "eu-central-1";
        return $"https://{subdomain}.hasura.{region}.nhost.run/v1/graphql";
    }

    public static string GetAuthEndpoint(string subdomain, string region = "eu-central-1")
    {
        if (string.IsNullOrWhiteSpace(region))
            region = "eu-central-1";
        return $"https://{subdomain}.auth.{region}.nhost.run/v1/auth";
    }

    public static string GetStorageEndpoint(string subdomain)
    {
        return $"https://{subdomain}.nhost.run/v1/storage";
    }
}