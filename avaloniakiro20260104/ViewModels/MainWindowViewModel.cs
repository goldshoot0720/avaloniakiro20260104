using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using avaloniakiro20260104.Models;
using avaloniakiro20260104.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

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

    [ObservableProperty]
    private SystemSettings _systemSettings = new();

    [ObservableProperty]
    private bool _isTestingConnection = false;

    [ObservableProperty]
    private string _connectionTestResult = "";

    [ObservableProperty]
    private FoodItem? _selectedFoodItem;

    [ObservableProperty]
    private Subscription? _selectedSubscription;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _statusMessage = "";

    private IApiService? _apiService;

    public MainWindowViewModel()
    {
        SystemSettings.LoadSettings();
        InitializeApiService();
        LoadData();
    }

    private void InitializeApiService()
    {
        try
        {
            if (SystemSettings?.NhostSubdomain != null && SystemSettings?.NhostAdminSecret != null)
            {
                // 根據設定選擇 API 類型
                var apiType = SystemSettings.UseGraphQL ? 
                    ApiServiceFactory.ApiType.GraphQL : 
                    ApiServiceFactory.ApiType.REST;
                
                _apiService = ApiServiceFactory.CreateApiService(
                    apiType,
                    SystemSettings.NhostSubdomain, 
                    SystemSettings.NhostAdminSecret
                );
                
                SystemSettings.CurrentApiType = SystemSettings.UseGraphQL ? "GraphQL" : "REST";
                StatusMessage = $"{SystemSettings.CurrentApiType} API 服務初始化成功";
            }
            else
            {
                StatusMessage = "API 設定不完整，使用本地模式";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"API 服務初始化失敗: {ex.Message}";
            Console.WriteLine($"InitializeApiService Exception: {ex}");
        }
    }

    private async void LoadData()
    {
        IsLoading = true;
        StatusMessage = "載入資料中...";

        try
        {
            // 嘗試從 API 載入資料
            if (_apiService != null)
            {
                await LoadDataFromApi();
            }
            else
            {
                // 如果 API 不可用，從本地載入
                LoadDataFromLocal();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"載入資料失敗: {ex.Message}";
            Console.WriteLine($"LoadData Exception: {ex}");
            // 載入本地資料作為備用
            try
            {
                LoadDataFromLocal();
            }
            catch (Exception localEx)
            {
                StatusMessage = $"本地資料載入也失敗: {localEx.Message}";
                Console.WriteLine($"LoadDataFromLocal Exception: {localEx}");
                // 如果連本地資料都載入失敗，初始化空集合
                InitializeEmptyCollections();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void InitializeEmptyCollections()
    {
        if (FoodItems == null)
            FoodItems = new ObservableCollection<FoodItem>();
        if (Subscriptions == null)
            Subscriptions = new ObservableCollection<Subscription>();
        if (Videos == null)
            Videos = new ObservableCollection<VideoContent>();
        
        StatusMessage = "已初始化空資料集合";
    }

    private async Task LoadDataFromApi()
    {
        if (_apiService == null) return;

        try
        {
            // 載入食品資料
            var foodItems = await _apiService.GetFoodItemsAsync();
            if (FoodItems == null)
                FoodItems = new ObservableCollection<FoodItem>();
            
            FoodItems.Clear();
            // 按到期日期排序：由近至遠（最近的日期在前面）
            var sortedFoodItems = foodItems.OrderBy(f => f.ToDate).ToList();
            foreach (var item in sortedFoodItems)
            {
                FoodItems.Add(item);
            }

            // 載入訂閱資料
            var subscriptions = await _apiService.GetSubscriptionsAsync();
            if (Subscriptions == null)
                Subscriptions = new ObservableCollection<Subscription>();
                
            Subscriptions.Clear();
            // 按下次付款日期排序：由近至遠（最近的日期在前面）
            var sortedSubscriptions = subscriptions.OrderBy(s => s.NextPaymentDate).ToList();
            foreach (var subscription in sortedSubscriptions)
            {
                Subscriptions.Add(subscription);
            }

            // 初始化影片資料
            InitializeSampleVideoData();

            StatusMessage = "資料載入成功";
        }
        catch (Exception ex)
        {
            StatusMessage = $"從 API 載入資料失敗: {ex.Message}";
            Console.WriteLine($"LoadDataFromApi Exception: {ex}");
            throw;
        }
    }

    private void LoadDataFromLocal()
    {
        // 載入儲存的資料
        var savedFoodItems = DataService.LoadFoodItems();
        var savedSubscriptions = DataService.LoadSubscriptions();

        if (savedFoodItems.Count > 0)
        {
            if (FoodItems == null)
                FoodItems = new ObservableCollection<FoodItem>();
            FoodItems.Clear();
            // 按到期日期排序：由近至遠（最近的日期在前面）
            var sortedFoodItems = savedFoodItems.OrderBy(f => f.ToDate).ToList();
            foreach (var item in sortedFoodItems)
            {
                FoodItems.Add(item);
            }
        }
        else
        {
            InitializeSampleFoodData();
        }

        if (savedSubscriptions.Count > 0)
        {
            if (Subscriptions == null)
                Subscriptions = new ObservableCollection<Subscription>();
            Subscriptions.Clear();
            // 按下次付款日期排序：由近至遠（最近的日期在前面）
            var sortedSubscriptions = savedSubscriptions.OrderBy(s => s.NextPaymentDate).ToList();
            foreach (var subscription in sortedSubscriptions)
            {
                Subscriptions.Add(subscription);
            }
        }
        else
        {
            InitializeSampleSubscriptionData();
        }

        // 初始化影片資料（暫時不持久化）
        InitializeSampleVideoData();
        
        StatusMessage = "從本地載入資料";
    }

    private void InitializeSampleFoodData()
    {
        // 按到期日期排序：由近至遠
        var sampleFoodItems = new List<FoodItem>
        {
            new FoodItem { Id = "1", Name = "【蛋糕】五香滷蛋休閒丸子", ToDate = DateTime.Now.AddDays(6), Amount = 3 },
            new FoodItem { Id = "2", Name = "【蛋糕】日式滷蛋休閒丸子", ToDate = DateTime.Now.AddDays(7), Amount = 6 },
            new FoodItem { Id = "3", Name = "樂事", ToDate = DateTime.Now.AddDays(22), Amount = 5 }
        };

        var sortedItems = sampleFoodItems.OrderBy(f => f.ToDate).ToList();
        foreach (var item in sortedItems)
        {
            FoodItems.Add(item);
        }
    }

    private void InitializeSampleSubscriptionData()
    {
        // 按下次付款日期排序：由近至遠
        var sampleSubscriptions = new List<Subscription>
        {
            new Subscription { Id = "1", Name = "kiro pro", NextPaymentDate = DateTime.Now.AddDays(1), Amount = 640, Category = "工作" },
            new Subscription { Id = "2", Name = "天氣/虛擬中心儲料", NextPaymentDate = DateTime.Now.AddDays(2), Amount = 530, Category = "雲端服務" },
            new Subscription { Id = "3", Name = "Netflix", NextPaymentDate = DateTime.Now.AddDays(11), Amount = 290, Category = "娛樂" }
        };

        var sortedSubscriptions = sampleSubscriptions.OrderBy(s => s.NextPaymentDate).ToList();
        foreach (var subscription in sortedSubscriptions)
        {
            Subscriptions.Add(subscription);
        }
    }

    private void InitializeSampleVideoData()
    {
        Videos.Add(new VideoContent { Id = 1, Title = "鋒兄的傳奇人生", Description = "一個關於愛情與夢想的勵志故事", Duration = "15:32" });
        Videos.Add(new VideoContent { Id = 2, Title = "鋒兄進化Show🔥", Description = "展現鋒兄的成長歷程與蛻變", Duration = "12:45" });
    }

    [RelayCommand]
    private void SelectMenuItem(string menuItem)
    {
        SelectedMenuItem = menuItem;
    }

    // 統計屬性
    public int TotalFoodItems => FoodItems?.Count ?? 0;
    public int ExpiringFoodItems => FoodItems?.Count(f => f.ToDate <= DateTime.Now.AddDays(7) && f.ToDate > DateTime.Now) ?? 0;
    public int ExpiredFoodItems => FoodItems?.Count(f => f.ToDate <= DateTime.Now) ?? 0;
    
    public int TotalSubscriptions => Subscriptions?.Count ?? 0;
    public int UpcomingSubscriptions => Subscriptions?.Count(s => s.NextPaymentDate <= DateTime.Now.AddDays(3)) ?? 0;
    public decimal MonthlyTotal => Subscriptions?.Sum(s => s.Amount) ?? 0;

    [RelayCommand]
    private void SaveSettings()
    {
        try
        {
            SystemSettings.SaveSettings();
            // 可以顯示儲存成功的訊息
            // 這裡可以加入通知或狀態更新
        }
        catch (Exception ex)
        {
            // 處理儲存錯誤
            Console.WriteLine($"儲存設定失敗: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ResetSettings()
    {
        SystemSettings = new SystemSettings();
        OnPropertyChanged(nameof(SystemSettings));
    }

    [RelayCommand]
    private async Task TestNhostConnection()
    {
        IsTestingConnection = true;
        ConnectionTestResult = "正在測試連接...";

        try
        {
            if (_apiService != null)
            {
                // 測試 API 連接
                var foodItems = await _apiService.GetFoodItemsAsync();
                var subscriptions = await _apiService.GetSubscriptionsAsync();
                
                ConnectionTestResult = $"✅ 連接成功！獲取到 {foodItems.Count} 個食品項目和 {subscriptions.Count} 個訂閱項目。";
                
                // 如果測試成功，重新載入資料
                if (foodItems.Count > 0 || subscriptions.Count > 0)
                {
                    await LoadDataFromApi();
                }
            }
            else
            {
                var isConnected = await NhostService.TestConnectionAsync(
                    SystemSettings.NhostSubdomain, 
                    SystemSettings.NhostAdminSecret
                );

                ConnectionTestResult = isConnected 
                    ? "✅ 基本連接成功！請點擊重新連接 API 來初始化服務。" 
                    : "❌ 連接失敗，請檢查 Subdomain 和 Admin Secret 是否正確。";
            }
        }
        catch (Exception ex)
        {
            ConnectionTestResult = $"❌ 連接測試發生錯誤: {ex.Message}";
            Console.WriteLine($"Connection test error: {ex}");
        }
        finally
        {
            IsTestingConnection = false;
        }
    }

    // 食品管理命令
    [RelayCommand]
    private async Task AddFoodItem()
    {
        var dialog = new Views.FoodItemDialog();
        var mainWindow = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<FoodItem?>(mainWindow!);
        
        if (result != null)
        {
            IsLoading = true;
            StatusMessage = "新增食品中...";

            try
            {
                if (_apiService != null)
                {
                    // 使用 API 創建
                    var createdItem = await _apiService.CreateFoodItemAsync(result);
                    if (createdItem != null)
                    {
                        FoodItems.Add(createdItem);
                        // 重新排序：按到期日期由近至遠
                        SortFoodItems();
                        StatusMessage = "食品新增成功";
                    }
                    else
                    {
                        StatusMessage = "食品新增失敗";
                    }
                }
                else
                {
                    // 本地創建
                    var maxId = FoodItems.Count > 0 ? 
                        FoodItems.Where(f => int.TryParse(f.Id, out _))
                                 .Select(f => int.Parse(f.Id))
                                 .DefaultIfEmpty(0)
                                 .Max() : 0;
                    result.Id = (maxId + 1).ToString();
                    FoodItems.Add(result);
                    // 重新排序：按到期日期由近至遠
                    SortFoodItems();
                    DataService.SaveFoodItems(FoodItems);
                    StatusMessage = "食品新增成功（本地）";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"新增食品失敗: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task EditFoodItem(FoodItem? foodItem)
    {
        if (foodItem == null) return;
        
        var dialog = new Views.FoodItemDialog(foodItem);
        var mainWindow = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<FoodItem?>(mainWindow!);
        
        if (result != null)
        {
            IsLoading = true;
            StatusMessage = "更新食品中...";

            try
            {
                if (_apiService != null)
                {
                    Console.WriteLine($"Attempting to update food item with ID: {foodItem.Id}");
                    Console.WriteLine($"Original item: {foodItem.Name}, New data: {result.Name}");
                    
                    // 使用 API 更新
                    var updatedItem = await _apiService.UpdateFoodItemAsync(foodItem.Id, result);
                    if (updatedItem != null)
                    {
                        var index = FoodItems.IndexOf(foodItem);
                        if (index >= 0)
                        {
                            // 確保 ID 正確
                            updatedItem.Id = foodItem.Id;
                            
                            FoodItems[index] = updatedItem;
                            StatusMessage = "食品更新成功";
                            
                            Console.WriteLine($"Successfully updated food item: {updatedItem.Name}");
                            
                            // 更新統計資料
                            OnPropertyChanged(nameof(TotalFoodItems));
                            OnPropertyChanged(nameof(ExpiringFoodItems));
                            OnPropertyChanged(nameof(ExpiredFoodItems));
                        }
                        else
                        {
                            StatusMessage = "找不到要更新的食品項目";
                            Console.WriteLine("Could not find food item in collection to update");
                        }
                    }
                    else
                    {
                        StatusMessage = "食品更新失敗 - API 返回空結果，但可能已成功更新";
                        Console.WriteLine("API returned null, but update might have succeeded. Refreshing data...");
                        
                        // 嘗試重新載入資料以確認更新是否成功
                        try
                        {
                            await LoadDataFromApi();
                            StatusMessage = "已重新載入資料，請檢查更新是否成功";
                        }
                        catch (Exception refreshEx)
                        {
                            Console.WriteLine($"Failed to refresh data: {refreshEx.Message}");
                            StatusMessage = "更新狀態不明，請手動重新整理";
                        }
                    }
                }
                else
                {
                    // 本地更新
                    var index = FoodItems.IndexOf(foodItem);
                    if (index >= 0)
                    {
                        result.Id = foodItem.Id;
                        FoodItems[index] = result;
                        DataService.SaveFoodItems(FoodItems);
                        StatusMessage = "食品更新成功（本地）";
                        
                        // 更新統計資料
                        OnPropertyChanged(nameof(TotalFoodItems));
                        OnPropertyChanged(nameof(ExpiringFoodItems));
                        OnPropertyChanged(nameof(ExpiredFoodItems));
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"更新食品失敗: {ex.Message}";
                Console.WriteLine($"EditFoodItem Exception: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task DeleteFoodItem(FoodItem? foodItem)
    {
        if (foodItem == null) return;

        // 顯示確認對話框
        var confirmDialog = new Views.ConfirmationDialog($"確定要刪除食品項目「{foodItem.Name}」嗎？");
        var mainWindow = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var confirmed = await confirmDialog.ShowDialog<bool>(mainWindow!);
        
        if (!confirmed) return;

        IsLoading = true;
        StatusMessage = "刪除食品中...";

        try
        {
            if (_apiService != null)
            {
                // 使用 API 刪除
                var success = await _apiService.DeleteFoodItemAsync(foodItem.Id);
                if (success)
                {
                    FoodItems.Remove(foodItem);
                    StatusMessage = "食品刪除成功";
                }
                else
                {
                    StatusMessage = "食品刪除失敗";
                }
            }
            else
            {
                // 本地刪除
                FoodItems.Remove(foodItem);
                DataService.SaveFoodItems(FoodItems);
                StatusMessage = "食品刪除成功（本地）";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"刪除食品失敗: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // 訂閱管理命令
    [RelayCommand]
    private async Task AddSubscription()
    {
        var dialog = new Views.SubscriptionDialog();
        var mainWindow = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<Subscription?>(mainWindow!);
        
        if (result != null)
        {
            IsLoading = true;
            StatusMessage = "新增訂閱中...";

            try
            {
                if (_apiService != null)
                {
                    // 使用 API 創建
                    var createdItem = await _apiService.CreateSubscriptionAsync(result);
                    if (createdItem != null)
                    {
                        Subscriptions.Add(createdItem);
                        // 重新排序：按下次付款日期由近至遠
                        SortSubscriptions();
                        StatusMessage = "訂閱新增成功";
                    }
                    else
                    {
                        StatusMessage = "訂閱新增失敗";
                    }
                }
                else
                {
                    // 本地創建
                    var maxId = Subscriptions.Count > 0 ? 
                        Subscriptions.Where(s => int.TryParse(s.Id, out _))
                                    .Select(s => int.Parse(s.Id))
                                    .DefaultIfEmpty(0)
                                    .Max() : 0;
                    result.Id = (maxId + 1).ToString();
                    Subscriptions.Add(result);
                    // 重新排序：按下次付款日期由近至遠
                    SortSubscriptions();
                    DataService.SaveSubscriptions(Subscriptions);
                    StatusMessage = "訂閱新增成功（本地）";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"新增訂閱失敗: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task EditSubscription(Subscription? subscription)
    {
        if (subscription == null) return;
        
        var dialog = new Views.SubscriptionDialog(subscription);
        var mainWindow = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<Subscription?>(mainWindow!);
        
        if (result != null)
        {
            IsLoading = true;
            StatusMessage = "更新訂閱中...";

            try
            {
                if (_apiService != null)
                {
                    // 使用 API 更新
                    var updatedItem = await _apiService.UpdateSubscriptionAsync(subscription.Id, result);
                    if (updatedItem != null)
                    {
                        var index = Subscriptions.IndexOf(subscription);
                        if (index >= 0)
                        {
                            Subscriptions[index] = updatedItem;
                        }
                        StatusMessage = "訂閱更新成功";
                    }
                    else
                    {
                        StatusMessage = "訂閱更新失敗";
                    }
                }
                else
                {
                    // 本地更新
                    var index = Subscriptions.IndexOf(subscription);
                    if (index >= 0)
                    {
                        result.Id = subscription.Id;
                        Subscriptions[index] = result;
                        DataService.SaveSubscriptions(Subscriptions);
                        StatusMessage = "訂閱更新成功（本地）";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"更新訂閱失敗: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async Task DeleteSubscription(Subscription? subscription)
    {
        if (subscription == null) return;

        // 顯示確認對話框
        var confirmDialog = new Views.ConfirmationDialog($"確定要刪除訂閱項目「{subscription.Name}」嗎？");
        var mainWindow = (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var confirmed = await confirmDialog.ShowDialog<bool>(mainWindow!);
        
        if (!confirmed) return;

        IsLoading = true;
        StatusMessage = "刪除訂閱中...";

        try
        {
            if (_apiService != null)
            {
                // 使用 API 刪除
                var success = await _apiService.DeleteSubscriptionAsync(subscription.Id);
                if (success)
                {
                    Subscriptions.Remove(subscription);
                    StatusMessage = "訂閱刪除成功";
                }
                else
                {
                    StatusMessage = "訂閱刪除失敗";
                }
            }
            else
            {
                // 本地刪除
                Subscriptions.Remove(subscription);
                DataService.SaveSubscriptions(Subscriptions);
                StatusMessage = "訂閱刪除成功（本地）";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"刪除訂閱失敗: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshData()
    {
        if (_apiService != null)
        {
            IsLoading = true;
            StatusMessage = "重新載入資料中...";
            
            try
            {
                // 強制從 API 重新載入資料
                await LoadDataFromApi();
                
                // 更新統計資料
                OnPropertyChanged(nameof(TotalFoodItems));
                OnPropertyChanged(nameof(ExpiringFoodItems));
                OnPropertyChanged(nameof(ExpiredFoodItems));
                OnPropertyChanged(nameof(TotalSubscriptions));
                OnPropertyChanged(nameof(UpcomingSubscriptions));
                OnPropertyChanged(nameof(MonthlyTotal));
            }
            catch (Exception ex)
            {
                StatusMessage = $"重新載入失敗: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        else
        {
            StatusMessage = "API 服務未初始化，請檢查設定";
        }
    }

    [RelayCommand]
    private async Task ReconnectApi()
    {
        InitializeApiService();
        StatusMessage = "API 服務已重新連接";
        
        // 如果 API 服務初始化成功，立即載入資料
        if (_apiService != null)
        {
            await RefreshData();
        }
    }

    [RelayCommand]
    private async Task SwitchToGraphQL()
    {
        IsLoading = true;
        StatusMessage = "切換到 GraphQL API...";

        try
        {
            // 釋放舊的 API 服務
            _apiService?.Dispose();
            
            // 創建 GraphQL API 服務
            if (SystemSettings?.NhostSubdomain != null && SystemSettings?.NhostAdminSecret != null)
            {
                _apiService = ApiServiceFactory.CreateApiService(
                    ApiServiceFactory.ApiType.GraphQL,
                    SystemSettings.NhostSubdomain,
                    SystemSettings.NhostAdminSecret
                );
                
                // 更新設定
                SystemSettings.UseGraphQL = true;
                SystemSettings.CurrentApiType = "GraphQL";
                SystemSettings.SaveSettings();
                
                // 重新載入資料
                await LoadDataFromApi();
                StatusMessage = "已切換到 GraphQL API";
            }
            else
            {
                StatusMessage = "API 設定不完整";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"切換到 GraphQL 失敗: {ex.Message}";
            Console.WriteLine($"SwitchToGraphQL Exception: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SwitchToREST()
    {
        IsLoading = true;
        StatusMessage = "切換到 REST API...";

        try
        {
            // 釋放舊的 API 服務
            _apiService?.Dispose();
            
            // 創建 REST API 服務
            if (SystemSettings?.NhostSubdomain != null && SystemSettings?.NhostAdminSecret != null)
            {
                _apiService = ApiServiceFactory.CreateApiService(
                    ApiServiceFactory.ApiType.REST,
                    SystemSettings.NhostSubdomain,
                    SystemSettings.NhostAdminSecret
                );
                
                // 更新設定
                SystemSettings.UseGraphQL = false;
                SystemSettings.CurrentApiType = "REST";
                SystemSettings.SaveSettings();
                
                // 重新載入資料
                await LoadDataFromApi();
                StatusMessage = "已切換到 REST API";
            }
            else
            {
                StatusMessage = "API 設定不完整";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"切換到 REST 失敗: {ex.Message}";
            Console.WriteLine($"SwitchToREST Exception: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 重新排序食品項目：按到期日期由近至遠
    /// </summary>
    private void SortFoodItems()
    {
        var sortedItems = FoodItems.OrderBy(f => f.ToDate).ToList();
        FoodItems.Clear();
        foreach (var item in sortedItems)
        {
            FoodItems.Add(item);
        }
    }

    /// <summary>
    /// 重新排序訂閱項目：按下次付款日期由近至遠
    /// </summary>
    private void SortSubscriptions()
    {
        var sortedItems = Subscriptions.OrderBy(s => s.NextPaymentDate).ToList();
        Subscriptions.Clear();
        foreach (var item in sortedItems)
        {
            Subscriptions.Add(item);
        }
    }
}