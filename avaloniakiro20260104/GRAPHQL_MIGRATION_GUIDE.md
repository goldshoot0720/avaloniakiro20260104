# GraphQL 遷移完成指南

## 🎉 遷移狀態：完成

你的應用程式已成功遷移到支援 GraphQL API！現在你可以在 GraphQL 和 REST API 之間自由切換。

## 📋 已完成的遷移項目

### ✅ 核心架構
- [x] 創建 `IApiService` 介面
- [x] 實作 `GraphQLService` 類別
- [x] 更新 `ApiService` 實作介面
- [x] 創建 `ApiServiceFactory` 工廠類別
- [x] 更新 `MainWindowViewModel` 使用新架構

### ✅ 功能支援
- [x] 食品管理 CRUD 操作 (GraphQL)
- [x] 訂閱管理 CRUD 操作 (GraphQL)
- [x] API 類型動態切換
- [x] 設定持久化
- [x] 錯誤處理和日誌記錄

### ✅ 使用者介面
- [x] API 類型切換命令
- [x] 狀態顯示更新
- [x] 設定保存功能

## 🚀 如何使用

### 1. 預設行為
應用程式現在預設使用 **GraphQL API**。首次啟動時會自動初始化 GraphQL 服務。

### 2. 切換 API 類型
你可以通過以下方式切換 API：

#### 程式化切換
```csharp
// 切換到 GraphQL
await viewModel.SwitchToGraphQLCommand.ExecuteAsync(null);

// 切換到 REST
await viewModel.SwitchToRESTCommand.ExecuteAsync(null);
```

#### 設定檔案
在 `SystemSettings` 中：
```csharp
SystemSettings.UseGraphQL = true;  // 使用 GraphQL
SystemSettings.UseGraphQL = false; // 使用 REST
```

### 3. 檢查當前 API 類型
```csharp
string currentApi = SystemSettings.CurrentApiType; // "GraphQL" 或 "REST"
bool isUsingGraphQL = SystemSettings.UseGraphQL;
```

## 🔧 GraphQL 優勢

### 相比 REST API 的改進
1. **完整欄位支援**: 支援所有資料欄位，包括 `photohash`, `nextdate` 等
2. **靈活查詢**: 可以指定需要的欄位
3. **批次操作**: 單一請求執行多個操作
4. **強型別**: 完整的 schema 驗證
5. **更好的錯誤處理**: 詳細的錯誤訊息

### GraphQL 查詢範例
```graphql
# 獲取食品項目
query GetFoodItems {
  food {
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

# 創建食品項目
mutation CreateFoodItem($object: food_insert_input!) {
  insert_food_one(object: $object) {
    id
    name
    amount
  }
}
```

## 📊 效能比較

| 功能 | REST API | GraphQL API |
|------|----------|-------------|
| 欄位支援 | 部分 | 完整 |
| 查詢靈活性 | 固定 | 高度靈活 |
| 網路請求 | 多次 | 單次 |
| 錯誤處理 | 基本 | 詳細 |
| 型別安全 | 中等 | 高 |

## 🛠️ 開發者指南

### 添加新的 GraphQL 操作

1. **在 GraphQLService 中添加方法**:
```csharp
public async Task<List<FoodItem>> GetFoodItemsByCategory(string category)
{
    const string query = @"
        query GetFoodItemsByCategory($category: String!) {
            food(where: {category: {_eq: $category}}) {
                id
                name
                amount
            }
        }";
    
    var variables = new { category };
    var result = await ExecuteAsync<FoodQueryResult>(query, variables);
    return result?.Food ?? new List<FoodItem>();
}
```

2. **在 IApiService 介面中添加方法**:
```csharp
Task<List<FoodItem>> GetFoodItemsByCategory(string category);
```

3. **在 ApiService (REST) 中實作**:
```csharp
public async Task<List<FoodItem>> GetFoodItemsByCategory(string category)
{
    // REST API 可能不支援此功能
    throw new NotSupportedException("REST API 不支援按分類查詢");
}
```

### 除錯 GraphQL 查詢

1. **啟用詳細日誌**:
   - 查看控制台輸出中的 GraphQL 請求和回應
   - 檢查錯誤訊息中的詳細資訊

2. **使用 GraphQL Playground**:
   - 訪問 `https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/console`
   - 在 GraphiQL 中測試查詢

3. **常見問題**:
   - 檢查欄位名稱是否正確
   - 確認變數類型匹配
   - 驗證權限設定

## 🧪 測試

### 單元測試
```csharp
[Fact]
public async Task GraphQLService_GetFoodItems_ShouldReturnList()
{
    var service = new GraphQLService("subdomain", "secret");
    var result = await service.GetFoodItemsAsync();
    Assert.NotNull(result);
}
```

### 整合測試
使用 `ApiServiceTests.cs` 中的測試來驗證兩種 API 的功能。

## 📈 監控和維護

### 效能監控
- 監控 GraphQL 查詢執行時間
- 追蹤 API 錯誤率
- 記錄使用者偏好的 API 類型

### 維護建議
1. **定期更新 GraphQL Schema**: 當後端 schema 變更時更新查詢
2. **監控 API 使用情況**: 了解哪種 API 更受歡迎
3. **效能優化**: 根據使用模式優化查詢

## 🔄 回滾計劃

如果需要回滾到純 REST API：

1. **更新預設設定**:
```csharp
SystemSettings.UseGraphQL = false;
```

2. **移除 GraphQL 相關檔案** (可選):
   - `GraphQLService.cs`
   - `ApiServiceFactory.cs`

3. **恢復原始 MainWindowViewModel**:
   - 直接使用 `ApiService` 而不是 `IApiService`

## 🎯 下一步建議

1. **監控使用情況**: 觀察 GraphQL 和 REST 的使用模式
2. **收集回饋**: 了解使用者對新 API 的體驗
3. **優化查詢**: 根據實際使用情況優化 GraphQL 查詢
4. **考慮 Subscription**: 如果需要即時更新，可以添加 GraphQL Subscription

## 📞 支援

如果遇到問題：
1. 檢查控制台日誌
2. 驗證 API 設定
3. 測試網路連接
4. 查看 Hasura 控制台

---

**恭喜！** 你的應用程式現在支援現代化的 GraphQL API，同時保持與 REST API 的向後相容性。🎉