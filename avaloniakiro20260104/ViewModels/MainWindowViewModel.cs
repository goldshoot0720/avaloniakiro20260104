using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using avaloniakiro20260104.Models;
using System;
using System.Linq;

namespace avaloniakiro20260104.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedMenuItem = "儀表板";

    [ObservableProperty]
    private ObservableCollection<FoodItem> _foodItems = new();

    [ObservableProperty]
    private ObservableCollection<Subscription> _subscriptions = new();

    [ObservableProperty]
    private ObservableCollection<VideoContent> _videos = new();

    public MainWindowViewModel()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        // 食品管理範例資料
        FoodItems.Add(new FoodItem { Id = 1, Name = "【蛋糕】五香滷蛋休閒丸子", ExpiryDate = DateTime.Now.AddDays(6), Quantity = 3 });
        FoodItems.Add(new FoodItem { Id = 2, Name = "【蛋糕】日式滷蛋休閒丸子", ExpiryDate = DateTime.Now.AddDays(7), Quantity = 6 });
        FoodItems.Add(new FoodItem { Id = 3, Name = "樂事", ExpiryDate = DateTime.Now.AddDays(22), Quantity = 5 });

        // 訂閱管理範例資料
        Subscriptions.Add(new Subscription { Id = 1, Name = "kiro pro", NextPaymentDate = DateTime.Now.AddDays(1), Amount = 640 });
        Subscriptions.Add(new Subscription { Id = 2, Name = "天氣/虛擬中心儲料", NextPaymentDate = DateTime.Now.AddDays(2), Amount = 530 });
        Subscriptions.Add(new Subscription { Id = 3, Name = "Netflix", NextPaymentDate = DateTime.Now.AddDays(11), Amount = 290 });

        // 影片內容範例資料
        Videos.Add(new VideoContent { Id = 1, Title = "鋒兄的傳奇人生", Description = "一個關於愛情與夢想的勵志故事", Duration = "15:32" });
        Videos.Add(new VideoContent { Id = 2, Title = "鋒兄進化Show🔥", Description = "展現鋒兄的成長歷程與蛻變", Duration = "12:45" });
    }

    [RelayCommand]
    private void SelectMenuItem(string menuItem)
    {
        SelectedMenuItem = menuItem;
    }

    // 統計屬性
    public int TotalFoodItems => FoodItems.Count;
    public int ExpiringFoodItems => FoodItems.Count(f => f.ExpiryDate <= DateTime.Now.AddDays(7) && f.ExpiryDate > DateTime.Now);
    public int ExpiredFoodItems => FoodItems.Count(f => f.ExpiryDate <= DateTime.Now);
    
    public int TotalSubscriptions => Subscriptions.Count;
    public int UpcomingSubscriptions => Subscriptions.Count(s => s.NextPaymentDate <= DateTime.Now.AddDays(3));
    public decimal MonthlyTotal => Subscriptions.Sum(s => s.Amount);
}