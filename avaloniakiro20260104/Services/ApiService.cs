using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _adminSecret;

    public ApiService(string subdomain, string adminSecret, string region = "eu-central-1")
    {
        if (string.IsNullOrWhiteSpace(subdomain))
            throw new ArgumentException("Subdomain cannot be null or empty", nameof(subdomain));
        if (string.IsNullOrWhiteSpace(adminSecret))
            throw new ArgumentException("Admin secret cannot be null or empty", nameof(adminSecret));
        if (string.IsNullOrWhiteSpace(region))
            region = "eu-central-1"; // 預設值

        _httpClient = new HttpClient();
        _baseUrl = $"https://{subdomain}.hasura.{region}.nhost.run/api/rest";
        _adminSecret = adminSecret;
        
        // 設定預設標頭 - 不要在這裡設定 Content-Type
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-hasura-admin-secret", _adminSecret);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        // 設定超時
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    private JsonSerializerOptions GetJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // 不使用 camelCase 轉換
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        // Add custom converters
        options.Converters.Add(new DateTimeConverter());
        options.Converters.Add(new FoodItemConverter());
        options.Converters.Add(new SubscriptionConverter());
        
        return options;
    }

    // 食品 API 操作
    public async Task<List<FoodItem>> GetFoodItemsAsync()
    {
        try
        {
            Console.WriteLine($"Requesting food items from: {_baseUrl}/food/");
            var response = await _httpClient.GetAsync($"{_baseUrl}/food/");
            
            Console.WriteLine($"Food API Response Status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Food API Response: {json}");
                
                // 解析包裝格式，支援多層嵌套
                var jsonDoc = JsonDocument.Parse(json);
                
                // 嘗試 {"data": {"food": [...]}} 格式
                if (jsonDoc.RootElement.TryGetProperty("data", out var dataElement) &&
                    dataElement.TryGetProperty("food", out var foodArray))
                {
                    Console.WriteLine($"Found nested food array with {foodArray.GetArrayLength()} items");
                    var result = JsonSerializer.Deserialize<List<FoodItem>>(foodArray.GetRawText(), GetJsonOptions());
                    Console.WriteLine($"Successfully deserialized {result?.Count ?? 0} food items from nested structure");
                    return result ?? new List<FoodItem>();
                }
                // 嘗試 {"food": [...]} 格式
                else if (jsonDoc.RootElement.TryGetProperty("food", out foodArray))
                {
                    Console.WriteLine($"Found food array with {foodArray.GetArrayLength()} items");
                    var result = JsonSerializer.Deserialize<List<FoodItem>>(foodArray.GetRawText(), GetJsonOptions());
                    Console.WriteLine($"Successfully deserialized {result?.Count ?? 0} food items");
                    return result ?? new List<FoodItem>();
                }
                else
                {
                    Console.WriteLine("No 'food' property found in response, trying direct deserialization");
                    // 嘗試直接解析為陣列
                    try
                    {
                        var result = JsonSerializer.Deserialize<List<FoodItem>>(json, GetJsonOptions());
                        Console.WriteLine($"Direct deserialization successful: {result?.Count ?? 0} items");
                        return result ?? new List<FoodItem>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Direct deserialization failed: {ex.Message}");
                    }
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Food API Error: {response.StatusCode} - {errorContent}");
            }
            
            return new List<FoodItem>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"獲取食品列表失敗: {ex.Message}");
            Console.WriteLine($"Exception Details: {ex}");
            return new List<FoodItem>();
        }
    }

    public async Task<FoodItem?> CreateFoodItemAsync(FoodItem foodItem)
    {
        try
        {
            if (foodItem == null)
                throw new ArgumentNullException(nameof(foodItem));

            // 創建一個新的對象，只包含後端支援的核心欄位 (不包含 photohash)
            var apiData = new
            {
                name = foodItem.Name,
                amount = foodItem.Amount,
                to_date = foodItem.ToDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                photo = foodItem.Photo,
                price = foodItem.Price,
                shop = foodItem.Shop
            };

            var json = JsonSerializer.Serialize(apiData, GetJsonOptions());
            Console.WriteLine($"Creating food item with data: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/food", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create food item response: {responseJson}");
                
                // 嘗試直接解析為 FoodItem 或從包裝格式中提取
                try
                {
                    var result = JsonSerializer.Deserialize<FoodItem>(responseJson, GetJsonOptions());
                    return result;
                }
                catch
                {
                    // 如果直接解析失敗，嘗試從包裝格式中提取
                    var jsonDoc = JsonDocument.Parse(responseJson);
                    if (jsonDoc.RootElement.TryGetProperty("food", out var foodElement))
                    {
                        var result = JsonSerializer.Deserialize<FoodItem>(foodElement.GetRawText(), GetJsonOptions());
                        return result;
                    }
                    return null;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create Food Item API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"創建食品項目失敗: {ex.Message}");
            Console.WriteLine($"Exception Details: {ex}");
            return null;
        }
    }

    public async Task<FoodItem?> UpdateFoodItemAsync(string id, FoodItem foodItem)
    {
        try
        {
            // 創建一個新的對象，只包含後端支援的核心欄位 (不包含 photohash)
            var apiData = new
            {
                name = foodItem.Name,
                amount = foodItem.Amount,
                to_date = foodItem.ToDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                photo = foodItem.Photo,
                price = foodItem.Price,
                shop = foodItem.Shop
            };

            var json = JsonSerializer.Serialize(apiData, GetJsonOptions());
            Console.WriteLine($"Updating food item {id} with data: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // nhost REST API 使用 POST 方法進行更新操作
            var response = await _httpClient.PostAsync($"{_baseUrl}/food/{id}", content);
            
            Console.WriteLine($"Update food item response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update food item response: {responseJson}");
                
                // 嘗試直接解析為 FoodItem 或從包裝格式中提取
                try
                {
                    var result = JsonSerializer.Deserialize<FoodItem>(responseJson, GetJsonOptions());
                    return result;
                }
                catch
                {
                    // 如果直接解析失敗，嘗試從包裝格式中提取
                    var jsonDoc = JsonDocument.Parse(responseJson);
                    if (jsonDoc.RootElement.TryGetProperty("food", out var foodElement))
                    {
                        var result = JsonSerializer.Deserialize<FoodItem>(foodElement.GetRawText(), GetJsonOptions());
                        return result;
                    }
                    
                    // 如果更新成功但沒有返回資料，返回原始物件
                    foodItem.Id = id;
                    return foodItem;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update Food Item API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"更新食品項目失敗: {ex.Message}");
            Console.WriteLine($"Exception Details: {ex}");
            return null;
        }
    }

    public async Task<bool> DeleteFoodItemAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/food/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"刪除食品項目失敗: {ex.Message}");
            return false;
        }
    }

    // 訂閱 API 操作
    public async Task<List<Subscription>> GetSubscriptionsAsync()
    {
        try
        {
            Console.WriteLine($"Requesting subscriptions from: {_baseUrl}/subscription/");
            var response = await _httpClient.GetAsync($"{_baseUrl}/subscription/");
            
            Console.WriteLine($"Subscription API Response Status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Subscription API Response: {json}");
                
                // 解析包裝格式，支援多層嵌套
                var jsonDoc = JsonDocument.Parse(json);
                
                // 嘗試 {"data": {"subscription": [...]}} 格式
                if (jsonDoc.RootElement.TryGetProperty("data", out var dataElement) &&
                    dataElement.TryGetProperty("subscription", out var subscriptionArray))
                {
                    Console.WriteLine($"Found nested subscription array with {subscriptionArray.GetArrayLength()} items");
                    var result = JsonSerializer.Deserialize<List<Subscription>>(subscriptionArray.GetRawText(), GetJsonOptions());
                    Console.WriteLine($"Successfully deserialized {result?.Count ?? 0} subscription items from nested structure");
                    return result ?? new List<Subscription>();
                }
                // 嘗試 {"subscription": [...]} 格式
                else if (jsonDoc.RootElement.TryGetProperty("subscription", out subscriptionArray))
                {
                    Console.WriteLine($"Found subscription array with {subscriptionArray.GetArrayLength()} items");
                    var result = JsonSerializer.Deserialize<List<Subscription>>(subscriptionArray.GetRawText(), GetJsonOptions());
                    Console.WriteLine($"Successfully deserialized {result?.Count ?? 0} subscription items");
                    return result ?? new List<Subscription>();
                }
                else
                {
                    Console.WriteLine("No 'subscription' property found in response, trying direct deserialization");
                    // 嘗試直接解析為陣列
                    try
                    {
                        var result = JsonSerializer.Deserialize<List<Subscription>>(json, GetJsonOptions());
                        Console.WriteLine($"Direct deserialization successful: {result?.Count ?? 0} items");
                        return result ?? new List<Subscription>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Direct deserialization failed: {ex.Message}");
                    }
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Subscription API Error: {response.StatusCode} - {errorContent}");
            }
            
            return new List<Subscription>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"獲取訂閱列表失敗: {ex.Message}");
            Console.WriteLine($"Exception Details: {ex}");
            return new List<Subscription>();
        }
    }

    public async Task<Subscription?> CreateSubscriptionAsync(Subscription subscription)
    {
        try
        {
            // 創建一個新的對象，只包含最基本的必需欄位
            var apiData = new
            {
                name = subscription.Name,
                price = subscription.Amount,
                site = subscription.Url,
                note = subscription.Description
            };

            var json = JsonSerializer.Serialize(apiData, GetJsonOptions());
            Console.WriteLine($"Creating subscription with data: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/subscription/", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create subscription response: {responseJson}");
                
                // 嘗試直接解析為 Subscription 或從包裝格式中提取
                try
                {
                    var result = JsonSerializer.Deserialize<Subscription>(responseJson, GetJsonOptions());
                    return result;
                }
                catch
                {
                    // 如果直接解析失敗，嘗試從包裝格式中提取
                    var jsonDoc = JsonDocument.Parse(responseJson);
                    if (jsonDoc.RootElement.TryGetProperty("subscription", out var subscriptionElement))
                    {
                        var result = JsonSerializer.Deserialize<Subscription>(subscriptionElement.GetRawText(), GetJsonOptions());
                        return result;
                    }
                    return null;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create Subscription API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"創建訂閱失敗: {ex.Message}");
            Console.WriteLine($"Exception Details: {ex}");
            return null;
        }
    }

    public async Task<Subscription?> UpdateSubscriptionAsync(string id, Subscription subscription)
    {
        try
        {
            // 創建一個新的對象，只包含最基本的必需欄位
            var apiData = new
            {
                name = subscription.Name,
                price = subscription.Amount,
                site = subscription.Url,
                note = subscription.Description
            };

            var json = JsonSerializer.Serialize(apiData, GetJsonOptions());
            Console.WriteLine($"Updating subscription {id} with data: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // nhost REST API 使用 POST 方法進行更新操作
            var response = await _httpClient.PostAsync($"{_baseUrl}/subscription/{id}", content);
            
            Console.WriteLine($"Update subscription response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update subscription response: {responseJson}");
                
                // 嘗試直接解析為 Subscription 或從包裝格式中提取
                try
                {
                    var result = JsonSerializer.Deserialize<Subscription>(responseJson, GetJsonOptions());
                    return result;
                }
                catch
                {
                    // 如果直接解析失敗，嘗試從包裝格式中提取
                    var jsonDoc = JsonDocument.Parse(responseJson);
                    if (jsonDoc.RootElement.TryGetProperty("subscription", out var subscriptionElement))
                    {
                        var result = JsonSerializer.Deserialize<Subscription>(subscriptionElement.GetRawText(), GetJsonOptions());
                        return result;
                    }
                    
                    // 如果更新成功但沒有返回資料，返回原始物件
                    subscription.Id = id;
                    return subscription;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update Subscription API Error: {response.StatusCode} - {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"更新訂閱失敗: {ex.Message}");
            Console.WriteLine($"Exception Details: {ex}");
            return null;
        }
    }

    public async Task<bool> DeleteSubscriptionAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/subscription/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"刪除訂閱失敗: {ex.Message}");
            return false;
        }
    }

    public async Task<FoodItem?> GetFoodItemByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/food/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<FoodItem>(json, GetJsonOptions());
                return result;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"獲取食品項目失敗: {ex.Message}");
            return null;
        }
    }

    public async Task<Subscription?> GetSubscriptionByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/subscription/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Subscription>(json, GetJsonOptions());
                return result;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"獲取訂閱項目失敗: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Custom DateTime converter to handle various date formats from API
public class DateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] DateFormats = new[]
    {
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "dd/MM/yyyy"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
            return DateTime.Now;

        // 嘗試標準解析
        if (DateTime.TryParse(dateString, out var result))
            return result;

        // 嘗試特定格式
        foreach (var format in DateFormats)
        {
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out result))
                return result;
        }

        Console.WriteLine($"Failed to parse date: {dateString}, using current time");
        return DateTime.Now;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}