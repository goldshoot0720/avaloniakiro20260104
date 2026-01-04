using System;

namespace avaloniakiro20260104.Services;

/// <summary>
/// API 服務工廠 - 用於創建不同類型的 API 服務
/// </summary>
public static class ApiServiceFactory
{
    public enum ApiType
    {
        REST,
        GraphQL
    }

    /// <summary>
    /// 創建 API 服務實例
    /// </summary>
    /// <param name="apiType">API 類型</param>
    /// <param name="subdomain">Hasura 子域名</param>
    /// <param name="adminSecret">管理員密鑰</param>
    /// <returns>API 服務實例</returns>
    public static IApiService CreateApiService(ApiType apiType, string subdomain, string adminSecret)
    {
        return apiType switch
        {
            ApiType.REST => new ApiService(subdomain, adminSecret),
            ApiType.GraphQL => new GraphQLService(subdomain, adminSecret),
            _ => throw new ArgumentException($"不支援的 API 類型: {apiType}")
        };
    }

    /// <summary>
    /// 根據設定創建 API 服務
    /// </summary>
    /// <param name="useGraphQL">是否使用 GraphQL</param>
    /// <param name="subdomain">Hasura 子域名</param>
    /// <param name="adminSecret">管理員密鑰</param>
    /// <returns>API 服務實例</returns>
    public static IApiService CreateApiService(bool useGraphQL, string subdomain, string adminSecret)
    {
        return CreateApiService(useGraphQL ? ApiType.GraphQL : ApiType.REST, subdomain, adminSecret);
    }
}