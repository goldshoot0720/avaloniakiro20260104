# 日期排序實現：由近至遠

## 🎯 實現目標
將應用程式中的食品和訂閱項目按日期排序，顯示順序為「由近至遠」（最近的日期在前面）。

## ✅ 已實現的功能

### 1. 資料載入時排序

#### API 資料載入 (`LoadDataFromApi`)
```csharp
// 食品項目：按到期日期排序
var sortedFoodItems = foodItems.OrderBy(f => f.ToDate).ToList();

// 訂閱項目：按下次付款日期排序
var sortedSubscriptions = subscriptions.OrderBy(s => s.NextPaymentDate).ToList();
```

#### 本地資料載入 (`LoadDataFromLocal`)
```csharp
// 食品項目：按到期日期排序
var sortedFoodItems = savedFoodItems.OrderBy(f => f.ToDate).ToList();

// 訂閱項目：按下次付款日期排序
var sortedSubscriptions = savedSubscriptions.OrderBy(s => s.NextPaymentDate).ToList();
```

### 2. GraphQL 查詢排序

#### 食品查詢
```graphql
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
}
```

#### 訂閱查詢
```graphql
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
}
```

### 3. 新增項目後自動排序

#### 食品新增後排序
```csharp
FoodItems.Add(createdItem);
SortFoodItems(); // 自動重新排序
```

#### 訂閱新增後排序
```csharp
Subscriptions.Add(createdItem);
SortSubscriptions(); // 自動重新排序
```

### 4. 輔助排序方法

```csharp
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
```

## 📊 排序邏輯說明

### 排序方式
- **食品項目**：按 `ToDate`（到期日期）升序排列
- **訂閱項目**：按 `NextPaymentDate`（下次付款日期）升序排列
- **結果**：最近的日期顯示在列表頂部

### 排序時機
1. **應用程式啟動時**：載入資料後自動排序
2. **重新整理資料時**：從 API 或本地載入後排序
3. **新增項目後**：添加新項目後立即重新排序
4. **API 切換後**：切換 GraphQL/REST 後重新載入並排序

## 🔧 技術實現細節

### 使用的排序方法
- `OrderBy(x => x.Date)`：LINQ 升序排序
- `ToList()`：轉換為 List 以便操作
- `Clear()` + `Add()`：更新 ObservableCollection

### GraphQL 排序語法
- `order_by: {field_name: asc}`：升序排序
- `order_by: {field_name: desc}`：降序排序（未使用）

### 為什麼使用升序 (asc)
- 升序排序讓較小的日期值（較早的日期）排在前面
- 對於即將到期的食品和即將付款的訂閱，這樣的排序最有意義
- 使用者可以優先看到需要立即處理的項目

## 📱 使用者體驗

### 食品管理
- 即將過期的食品顯示在頂部
- 使用者可以優先處理快過期的食品
- 新增食品後自動按到期日排序

### 訂閱管理
- 即將付款的訂閱顯示在頂部
- 使用者可以提前準備付款
- 新增訂閱後自動按付款日排序

## 🧪 測試建議

### 手動測試
1. **新增不同日期的項目**：驗證排序是否正確
2. **重新整理資料**：確認排序保持一致
3. **切換 API 類型**：驗證 GraphQL 和 REST 都正確排序

### 自動化測試
```csharp
[Test]
public void SortFoodItems_ShouldOrderByDateAscending()
{
    // 安排測試資料
    var items = new List<FoodItem>
    {
        new FoodItem { ToDate = DateTime.Now.AddDays(5) },
        new FoodItem { ToDate = DateTime.Now.AddDays(1) },
        new FoodItem { ToDate = DateTime.Now.AddDays(3) }
    };
    
    // 執行排序
    var sorted = items.OrderBy(f => f.ToDate).ToList();
    
    // 驗證結果
    Assert.That(sorted[0].ToDate, Is.LessThan(sorted[1].ToDate));
    Assert.That(sorted[1].ToDate, Is.LessThan(sorted[2].ToDate));
}
```

## 🔄 未來改進建議

### 1. 可配置排序
```csharp
public enum SortOrder { Ascending, Descending }
public SortOrder FoodSortOrder { get; set; } = SortOrder.Ascending;
```

### 2. 多欄位排序
```csharp
// 先按日期，再按名稱排序
var sorted = items.OrderBy(f => f.ToDate).ThenBy(f => f.Name);
```

### 3. 使用者自定義排序
- 允許使用者選擇排序欄位
- 允許使用者選擇升序/降序
- 記住使用者的排序偏好

## 📈 效能考量

### 目前實現
- 使用 LINQ `OrderBy`：O(n log n) 時間複雜度
- 適合中小型資料集（< 1000 項目）

### 優化建議
- 對於大型資料集，考慮在資料庫層面排序
- 使用 GraphQL 的 `order_by` 減少客戶端排序
- 考慮虛擬化列表以提高 UI 效能

---

**總結**：日期排序功能已完全實現，確保使用者始終看到按時間優先級排列的項目，提升了應用程式的實用性和使用者體驗。