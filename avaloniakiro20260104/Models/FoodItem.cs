using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniakiro20260104.Models;

public partial class FoodItem : ObservableObject
{
    [ObservableProperty]
    [JsonPropertyName("id")]
    private string _id = string.Empty;

    [ObservableProperty]
    [JsonPropertyName("name")]
    private string _name = string.Empty;

    [ObservableProperty]
    [JsonPropertyName("to_date")]
    private DateTime _expiryDate = DateTime.Now.AddDays(7);

    [ObservableProperty]
    [JsonPropertyName("amount")]
    private int _quantity = 1;

    [ObservableProperty]
    [JsonPropertyName("photo")]
    private string _imageUrl = string.Empty;

    [ObservableProperty]
    [JsonPropertyName("shop")]
    private string? _shop;

    [ObservableProperty]
    [JsonPropertyName("price")]
    private decimal? _price;

    // 本地屬性（不從 API 映射）
    [JsonIgnore]
    public string Category { get; set; } = "其他";

    [JsonIgnore]
    public string Location { get; set; } = "冰箱";

    [JsonIgnore]
    public string Notes { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string Status => ExpiryDate < DateTime.Now ? "已過期" : 
                           ExpiryDate <= DateTime.Now.AddDays(7) ? "即將過期" : "正常";

    [JsonIgnore]
    public string StatusColor => Status switch
    {
        "已過期" => "#EF4444",
        "即將過期" => "#F59E0B",
        _ => "#10B981"
    };

    [JsonIgnore]
    public int DaysUntilExpiry => (ExpiryDate - DateTime.Now).Days;
}