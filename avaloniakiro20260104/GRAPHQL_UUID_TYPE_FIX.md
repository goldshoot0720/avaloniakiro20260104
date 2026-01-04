# GraphQL UUID 類型修復

## 問題描述
在使用 GraphQL API 進行更新、刪除和查詢操作時出現錯誤：
```
GraphQL Error: variable 'id' is declared as 'String!', but used where 'uuid!' is expected
```

## 問題原因
GraphQL 變數聲明中使用了錯誤的類型：
- 使用了 `String!` 類型聲明 ID 變數
- 但 Hasura GraphQL API 期望 `uuid!` 類型

## 解決方案
修改 `GraphQLService.cs` 中所有涉及 ID 參數的 GraphQL 查詢和變更操作：

### 修復的操作
1. **食品項目操作**：
   - `GetFoodItemByIdAsync`: `$id: String!` → `$id: uuid!`
   - `UpdateFoodItemAsync`: `$id: String!` → `$id: uuid!`
   - `DeleteFoodItemAsync`: `$id: String!` → `$id: uuid!`

2. **訂閱項目操作**：
   - `GetSubscriptionByIdAsync`: `$id: String!` → `$id: uuid!`
   - `UpdateSubscriptionAsync`: `$id: uuid!` → `$id: uuid!`
   - `DeleteSubscriptionAsync`: `$id: String!` → `$id: uuid!`

### 修復前後對比
```graphql
// 修復前（錯誤）
mutation UpdateFoodItem($id: String!, $changes: food_set_input!) {
    update_food_by_pk(pk_columns: {id: $id}, _set: $changes) {
        id
        name
        ...
    }
}

// 修復後（正確）
mutation UpdateFoodItem($id: uuid!, $changes: food_set_input!) {
    update_food_by_pk(pk_columns: {id: $id}, _set: $changes) {
        id
        name
        ...
    }
}
```

## 測試結果
修復後的應用程式：
- ✅ 成功載入食品和訂閱資料
- ✅ 可以正常執行更新操作
- ✅ 可以正常執行刪除操作
- ✅ 可以正常執行單項查詢操作

## 相關檔案
- `avaloniakiro20260104/Services/GraphQLService.cs` - GraphQL 服務實現

## 修復日期
2026-01-04

## 注意事項
此修復確保了 GraphQL 變數類型與 Hasura API 期望的類型完全匹配，避免了類型不匹配導致的運行時錯誤。