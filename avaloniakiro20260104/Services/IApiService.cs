using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

/// <summary>
/// API 服務介面 - 支援 REST 和 GraphQL 實作
/// </summary>
public interface IApiService : IDisposable
{
    // 食品管理 API
    Task<List<FoodItem>> GetFoodItemsAsync();
    Task<FoodItem?> GetFoodItemByIdAsync(string id);
    Task<FoodItem?> CreateFoodItemAsync(FoodItem foodItem);
    Task<FoodItem?> UpdateFoodItemAsync(string id, FoodItem foodItem);
    Task<bool> DeleteFoodItemAsync(string id);

    // 訂閱管理 API
    Task<List<Subscription>> GetSubscriptionsAsync();
    Task<Subscription?> GetSubscriptionByIdAsync(string id);
    Task<Subscription?> CreateSubscriptionAsync(Subscription subscription);
    Task<Subscription?> UpdateSubscriptionAsync(string id, Subscription subscription);
    Task<bool> DeleteSubscriptionAsync(string id);
}