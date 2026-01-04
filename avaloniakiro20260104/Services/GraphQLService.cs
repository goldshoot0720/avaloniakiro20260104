using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

/// <summary>
/// GraphQL API 服務 - 作為 REST API 的替代方案
/// </summary>
public class GraphQLService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly string _adminSecret;

    public GraphQLService(string subdomain, string adminSecret)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
            throw new ArgumentException("Subdomain cannot be null or empty", nameof(subdomain));
        if (string.IsNullOrWhiteSpace(adminSecret))
            throw new ArgumentException("Admin secret cannot be null or empty", nameof(adminSecret));

        _httpClient = new HttpClient();
        _endpoint = $"https://{subdomain}.hasura.eu-central-1.nhost.run/v1/graphql";
        _adminSecret = adminSecret;
        
        // 設定預設標頭
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-hasura-admin-secret", _adminSecret);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    private JsonSerializerOptions GetJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        // Add custom converters
        options.Converters.Add(new DateTimeConverter());
        options.Converters.Add(new FoodItemConverter());
        options.Converters.Add(new SubscriptionConverter());
        
        return options;
    }

    /// <summary>
    /// 執行 GraphQL 查詢
    /// </summary>
    private async Task<T> ExecuteAsync<T>(string query, object variables = null)
    {
        try
        {
            var request = new
            {
                query = query,
                variables = variables
            };

            var json = JsonSerializer.Serialize(request, GetJsonOptions());
            Console.WriteLine($"GraphQL Request: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GraphQL Response: {responseJson}");
                
                var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(responseJson, GetJsonOptions());
                
                if (result.Errors != null && result.Errors.Count > 0)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Message));
                    throw new Exception($"GraphQL Error: {errorMessage}");
                }
                
                return result.Data;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"HTTP Error: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GraphQL 執行失敗: {ex.Message}");
            throw;
        }
    }

    #region 食品管理 GraphQL 操作

    /// <summary>
    /// 獲取所有食品項目（按到期日期排序：由近至遠）
    /// </summary>
    public async Task<List<FoodItem>> GetFoodItemsAsync()
    {
        const string query = @"
            query GetFoodItems {
                food(order_by: {to_date: asc}) {
                    id
                    name
                    amount
                    to_date
                    photo
                    photohash
                    price
                    shop
                }
            }";

        var result = await ExecuteAsync<FoodQueryResult>(query);
        return result?.Food ?? new List<FoodItem>();
    }

    /// <summary>
    /// 根據 ID 獲取食品項目
    /// </summary>
    public async Task<FoodItem?> GetFoodItemByIdAsync(string id)
    {
        const string query = @"
            query GetFoodItemById($id: String!) {
                food_by_pk(id: $id) {
                    id
                    name
                    amount
                    to_date
                    photo
                    photohash
                    price
                    shop
                }
            }";

        var variables = new { id };
        var result = await ExecuteAsync<FoodByIdResult>(query, variables);
        return result?.FoodByPk;
    }

    /// <summary>
    /// 創建食品項目
    /// </summary>
    public async Task<FoodItem?> CreateFoodItemAsync(FoodItem foodItem)
    {
        const string mutation = @"
            mutation CreateFoodItem($object: food_insert_input!) {
                insert_food_one(object: $object) {
                    id
                    name
                    amount
                    to_date
                    photo
                    photohash
                    price
                    shop
                }
            }";

        var variables = new
        {
            @object = new
            {
                name = foodItem.Name,
                amount = foodItem.Amount,
                to_date = foodItem.ToDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                photo = foodItem.Photo,
                photohash = foodItem.PhotoHash,
                price = foodItem.Price,
                shop = foodItem.Shop
            }
        };

        var result = await ExecuteAsync<CreateFoodResult>(mutation, variables);
        return result?.InsertFoodOne;
    }

    /// <summary>
    /// 更新食品項目
    /// </summary>
    public async Task<FoodItem?> UpdateFoodItemAsync(string id, FoodItem foodItem)
    {
        const string mutation = @"
            mutation UpdateFoodItem($id: String!, $changes: food_set_input!) {
                update_food_by_pk(pk_columns: {id: $id}, _set: $changes) {
                    id
                    name
                    amount
                    to_date
                    photo
                    photohash
                    price
                    shop
                }
            }";

        var variables = new
        {
            id,
            changes = new
            {
                name = foodItem.Name,
                amount = foodItem.Amount,
                to_date = foodItem.ToDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                photo = foodItem.Photo,
                photohash = foodItem.PhotoHash,
                price = foodItem.Price,
                shop = foodItem.Shop
            }
        };

        var result = await ExecuteAsync<UpdateFoodResult>(mutation, variables);
        return result?.UpdateFoodByPk;
    }

    /// <summary>
    /// 刪除食品項目
    /// </summary>
    public async Task<bool> DeleteFoodItemAsync(string id)
    {
        const string mutation = @"
            mutation DeleteFoodItem($id: String!) {
                delete_food_by_pk(id: $id) {
                    id
                }
            }";

        var variables = new { id };
        var result = await ExecuteAsync<DeleteFoodResult>(mutation, variables);
        return result?.DeleteFoodByPk != null;
    }

    #endregion

    #region 訂閱管理 GraphQL 操作

    /// <summary>
    /// 獲取所有訂閱項目（按下次付款日期排序：由近至遠）
    /// </summary>
    public async Task<List<Subscription>> GetSubscriptionsAsync()
    {
        const string query = @"
            query GetSubscriptions {
                subscription(order_by: {nextdate: asc}) {
                    id
                    name
                    nextdate
                    price
                    site
                    note
                    account
                }
            }";

        var result = await ExecuteAsync<SubscriptionQueryResult>(query);
        return result?.Subscription ?? new List<Subscription>();
    }

    /// <summary>
    /// 根據 ID 獲取訂閱項目
    /// </summary>
    public async Task<Subscription?> GetSubscriptionByIdAsync(string id)
    {
        const string query = @"
            query GetSubscriptionById($id: String!) {
                subscription_by_pk(id: $id) {
                    id
                    name
                    nextdate
                    price
                    site
                    note
                    account
                }
            }";

        var variables = new { id };
        var result = await ExecuteAsync<SubscriptionByIdResult>(query, variables);
        return result?.SubscriptionByPk;
    }
    /// <summary>
    /// 創建訂閱項目
    /// </summary>
    public async Task<Subscription?> CreateSubscriptionAsync(Subscription subscription)
    {
        const string mutation = @"
            mutation CreateSubscription($object: subscription_insert_input!) {
                insert_subscription_one(object: $object) {
                    id
                    name
                    nextdate
                    price
                    site
                    note
                    account
                }
            }";

        var variables = new
        {
            @object = new
            {
                name = subscription.Name,
                nextdate = subscription.NextPaymentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                price = subscription.Amount,
                site = subscription.Url,
                note = subscription.Description,
                account = subscription.Account
            }
        };

        var result = await ExecuteAsync<CreateSubscriptionResult>(mutation, variables);
        return result?.InsertSubscriptionOne;
    }

    /// <summary>
    /// 更新訂閱項目
    /// </summary>
    public async Task<Subscription?> UpdateSubscriptionAsync(string id, Subscription subscription)
    {
        const string mutation = @"
            mutation UpdateSubscription($id: String!, $changes: subscription_set_input!) {
                update_subscription_by_pk(pk_columns: {id: $id}, _set: $changes) {
                    id
                    name
                    nextdate
                    price
                    site
                    note
                    account
                }
            }";

        var variables = new
        {
            id,
            changes = new
            {
                name = subscription.Name,
                nextdate = subscription.NextPaymentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                price = subscription.Amount,
                site = subscription.Url,
                note = subscription.Description,
                account = subscription.Account
            }
        };

        var result = await ExecuteAsync<UpdateSubscriptionResult>(mutation, variables);
        return result?.UpdateSubscriptionByPk;
    }

    /// <summary>
    /// 刪除訂閱項目
    /// </summary>
    public async Task<bool> DeleteSubscriptionAsync(string id)
    {
        const string mutation = @"
            mutation DeleteSubscription($id: String!) {
                delete_subscription_by_pk(id: $id) {
                    id
                }
            }";

        var variables = new { id };
        var result = await ExecuteAsync<DeleteSubscriptionResult>(mutation, variables);
        return result?.DeleteSubscriptionByPk != null;
    }

    #endregion

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

#region GraphQL 回應模型

public class GraphQLResponse<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; }
    
    [JsonPropertyName("errors")]
    public List<GraphQLError> Errors { get; set; }
}

public class GraphQLError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("path")]
    public List<object> Path { get; set; }
}

// 食品相關回應模型
public class FoodQueryResult
{
    [JsonPropertyName("food")]
    public List<FoodItem> Food { get; set; }
}

public class FoodByIdResult
{
    [JsonPropertyName("food_by_pk")]
    public FoodItem FoodByPk { get; set; }
}

public class CreateFoodResult
{
    [JsonPropertyName("insert_food_one")]
    public FoodItem InsertFoodOne { get; set; }
}

public class UpdateFoodResult
{
    [JsonPropertyName("update_food_by_pk")]
    public FoodItem UpdateFoodByPk { get; set; }
}

public class DeleteFoodResult
{
    [JsonPropertyName("delete_food_by_pk")]
    public FoodItem DeleteFoodByPk { get; set; }
}

// 訂閱相關回應模型
public class SubscriptionQueryResult
{
    [JsonPropertyName("subscription")]
    public List<Subscription> Subscription { get; set; }
}

public class SubscriptionByIdResult
{
    [JsonPropertyName("subscription_by_pk")]
    public Subscription SubscriptionByPk { get; set; }
}

public class CreateSubscriptionResult
{
    [JsonPropertyName("insert_subscription_one")]
    public Subscription InsertSubscriptionOne { get; set; }
}

public class UpdateSubscriptionResult
{
    [JsonPropertyName("update_subscription_by_pk")]
    public Subscription UpdateSubscriptionByPk { get; set; }
}

public class DeleteSubscriptionResult
{
    [JsonPropertyName("delete_subscription_by_pk")]
    public Subscription DeleteSubscriptionByPk { get; set; }
}

#endregion