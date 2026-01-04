using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniakiro20260104.Models;

public partial class Subscription : ObservableObject
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private DateTime _nextPaymentDate = DateTime.Now.AddMonths(1);

    [ObservableProperty]
    private decimal _amount = 0;

    [ObservableProperty]
    private string _url = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _account;

    // 本地屬性（不同步到後端）
    [JsonIgnore]
    public string Category { get; set; } = "娛樂";

    [JsonIgnore]
    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public string PaymentMethod { get; set; } = "信用卡";

    [JsonIgnore]
    public int BillingCycle { get; set; } = 1;

    [JsonIgnore]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string Status => !IsActive ? "已停用" :
                           NextPaymentDate <= DateTime.Now ? "需要付款" :
                           NextPaymentDate <= DateTime.Now.AddDays(3) ? "即將到期" : "正常";

    [JsonIgnore]
    public string StatusColor => Status switch
    {
        "已停用" => "#6B7280",
        "需要付款" => "#EF4444",
        "即將到期" => "#F59E0B",
        _ => "#10B981"
    };

    [JsonIgnore]
    public int DaysUntilPayment => (NextPaymentDate - DateTime.Now).Days;
}