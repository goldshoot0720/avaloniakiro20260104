# 訂閱顯示問題修復

## 問題描述
訂閱管理功能中出現顯示錯誤：
- 下次付款日期全部顯示為 2026-02-04（錯誤）
- 價錢全部顯示為 0（錯誤）

## 問題原因
API 返回的 JSON 欄位名稱與 C# 模型屬性名稱不匹配：
- API 使用 `nextdate` 但模型期望 `NextPaymentDate`
- API 使用 `price` 但模型期望 `Amount`
- API 使用 `site` 但模型期望 `Url`
- API 使用 `note` 但模型期望 `Description`

## 解決方案
創建自定義 JSON 轉換器 `SubscriptionConverter` 來處理欄位映射：

### 1. 移除 JsonPropertyName 屬性
從 `Subscription.cs` 模型中移除了 JsonPropertyName 屬性，避免衝突。

### 2. 創建自定義轉換器
`Services/SubscriptionConverter.cs` 實現了：
- `nextdate` → `NextPaymentDate` 的日期解析
- `price` → `Amount` 的數值轉換
- `site` → `Url` 的字串映射
- `note` → `Description` 的字串映射
- `account` → `Account` 的字串映射

### 3. 註冊轉換器
在 `GraphQLService.cs` 的 `GetJsonOptions()` 方法中註冊轉換器：
```csharp
options.Converters.Add(new SubscriptionConverter());
```

### 4. 修復編譯錯誤
在 `Subscription.cs` 中添加缺失的 using 語句：
```csharp
using System.Text.Json.Serialization;
```

## 測試結果
修復後的應用程式正確顯示：
- 各種不同的下次付款日期（2025-12-26, 2026-01-01, 2026-01-03 等）
- 正確的價格金額（530, 640, 129, 156 等）
- 所有訂閱項目按日期由近至遠排序

## 相關檔案
- `avaloniakiro20260104/Models/Subscription.cs` - 訂閱模型
- `avaloniakiro20260104/Services/SubscriptionConverter.cs` - 自定義轉換器
- `avaloniakiro20260104/Services/GraphQLService.cs` - GraphQL 服務
- `avaloniakiro20260104/ViewModels/MainWindowViewModel.cs` - 主視窗視圖模型

## 修復日期
2026-01-04