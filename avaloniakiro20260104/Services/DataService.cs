using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

public static class DataService
{
    private static readonly string DataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "鋒兄Next資訊管理"
    );

    private static readonly string FoodItemsPath = Path.Combine(DataDirectory, "fooditems.json");
    private static readonly string SubscriptionsPath = Path.Combine(DataDirectory, "subscriptions.json");

    static DataService()
    {
        if (!Directory.Exists(DataDirectory))
        {
            Directory.CreateDirectory(DataDirectory);
        }
    }

    public static ObservableCollection<FoodItem> LoadFoodItems()
    {
        try
        {
            if (File.Exists(FoodItemsPath))
            {
                var json = File.ReadAllText(FoodItemsPath);
                var items = JsonSerializer.Deserialize<FoodItem[]>(json);
                return new ObservableCollection<FoodItem>(items ?? Array.Empty<FoodItem>());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"載入食品資料時發生錯誤: {ex.Message}");
        }

        return new ObservableCollection<FoodItem>();
    }

    public static void SaveFoodItems(ObservableCollection<FoodItem> foodItems)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(foodItems.ToArray(), options);
            File.WriteAllText(FoodItemsPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"儲存食品資料時發生錯誤: {ex.Message}");
            throw;
        }
    }

    public static ObservableCollection<Subscription> LoadSubscriptions()
    {
        try
        {
            if (File.Exists(SubscriptionsPath))
            {
                var json = File.ReadAllText(SubscriptionsPath);
                var items = JsonSerializer.Deserialize<Subscription[]>(json);
                return new ObservableCollection<Subscription>(items ?? Array.Empty<Subscription>());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"載入訂閱資料時發生錯誤: {ex.Message}");
        }

        return new ObservableCollection<Subscription>();
    }

    public static void SaveSubscriptions(ObservableCollection<Subscription> subscriptions)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(subscriptions.ToArray(), options);
            File.WriteAllText(SubscriptionsPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"儲存訂閱資料時發生錯誤: {ex.Message}");
            throw;
        }
    }
}