using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _adminSecret;

    public ApiService(string subdomain, string adminSecret)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
            throw new ArgumentException("Subdomain cannot be null or empty", nameof(subdomain));
        if (string.IsNullOrWhiteSpace(adminSecret))
            throw new ArgumentException("Admin secret cannot be null or empty", nameof(adminSecret));

        _httpClient = new HttpClient();
        _baseUrl = $"https://{subdomain}.hasura.eu-central-1.nhost.run/api/rest";
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
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    // 食品 API 操作
    public async Task<List<FoodItem>> GetFoodItemsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/food/");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Food API Response: {json}");
                
                // 解析包裝格式 {"food": [...]}
                var jsonDoc = JsonDocument.Parse(json);
                if (jsonDoc.RootElement.TryGetProperty("food", out var foodArray))
                {
                    var result = JsonSerializer.Deserialize<List<FoodItem>>(foodArray.GetRawText(), GetJsonOptions());
                    return result ?? new List<FoodItem>();
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

            // 創建一個新的對象，只包含 API 需要的字段
            var apiData = new
            {
                name = foodItem.Name,
                amount = foodItem.Quantity,
                to_date = foodItem.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                photo = foodItem.ImageUrl,
                shop = foodItem.Shop,
                price = foodItem.Price
            };

            var json = JsonSerializer.Serialize(apiData, GetJsonOptions());
            Console.WriteLine($"Creating food item with data: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/food/", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create food response: {responseJson}");
                
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
                Console.WriteLine($"Create Food API Error: {response.StatusCode} - {errorContent}");
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
            // 創建一個新的對象，只包含 API 需要的字段
            var apiData = new
            {
                name = foodItem.Name,
                amount = foodItem.Quantity,
                to_date = foodItem.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                photo = foodItem.ImageUrl,
                shop = foodItem.Shop,
                price = foodItem.Price
            };

            var json = JsonSerializer.Serialize(apiData, GetJsonOptions());
            Console.WriteLine($"Updating food item {id} with data: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // nhost REST API 使用 POST 方法進行更新操作
            var response = await _httpClient.PostAsync($"{_baseUrl}/food/{id}", content);
            
            Console.WriteLine($"Update response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update food response: {responseJson}");
                
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
                    foodItem.Id = id; // 確保 ID 正確
                    return foodItem;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update Food API Error: {response.StatusCode} - {errorContent}");
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
            var response = await _httpClient.GetAsync($"{_baseUrl}/subscription/");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Subscription API Response: {json}");
                
                // 解析包裝格式 {"subscription": [...]}
                var jsonDoc = JsonDocument.Parse(json);
                if (jsonDoc.RootElement.TryGetProperty("subscription", out var subscriptionArray))
                {
                    var result = JsonSerializer.Deserialize<List<Subscription>>(subscriptionArray.GetRawText(), GetJsonOptions());
                    return result ?? new List<Subscription>();
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
            // 創建一個新的對象，只包含 API 需要的字段
            var apiData = new
            {
                name = subscription.Name,
                nextdate = subscription.NextPaymentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                price = subscription.Amount,
                site = subscription.Url,
                note = subscription.Description,
                account = subscription.Account
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
            // 創建一個新的對象，只包含 API 需要的字段
            var apiData = new
            {
                name = subscription.Name,
                nextdate = subscription.NextPaymentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                price = subscription.Amount,
                site = subscription.Url,
                note = subscription.Description,
                account = subscription.Account
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

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}