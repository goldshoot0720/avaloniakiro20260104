# REST API 整合說明

## 概述
此應用程式已整合 nhost REST API，支援食品和訂閱管理的完整 CRUD 操作。

## API 端點

### 基礎 URL
`https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest`

### 訂閱管理端點
- **GET** `/subscription` - 獲取所有訂閱項目
- **POST** `/subscription` - 創建新訂閱項目 (insert_subscription_one)
- **GET** `/subscription/:id` - 獲取指定訂閱項目 (subscription_by_pk)
- **POST** `/subscription/:id` - 更新指定訂閱項目 (update_subscription_by_pk)
- **DELETE** `/subscription/:id` - 刪除指定訂閱項目 (delete_subscription_by_pk)

### 食品管理端點
- **GET** `/food` - 獲取所有食品項目
- **POST** `/food` - 創建新食品項目 (insert_food_one)
- **GET** `/food/:id` - 獲取指定食品項目 (food_by_pk)
- **POST** `/food/:id` - 更新指定食品項目 (update_food_by_pk)
- **DELETE** `/food/:id` - 刪除指定食品項目 (delete_food_by_pk)

**注意：** 
- nhost REST API 使用 POST 方法進行更新操作，而不是傳統的 PUT/PATCH 方法
- 所有端點都是 GraphQL 請求的 REST 包裝器
- 需要在請求標頭中包含 `x-hasura-admin-secret` 進行認證

## 功能特色

### 🔄 自動切換模式
- **線上模式**：當 API 可用時，所有操作都會同步到 nhost 後端
- **離線模式**：當 API 不可用時，自動切換到本地儲存模式
- **智能重試**：提供重新連接 API 的功能

### 📊 食品管理 API
- **GET** `/food` - 獲取所有食品項目
- **GET** `/food/{id}` - 獲取指定食品項目
- **POST** `/food` - 創建新食品項目
- **POST** `/food/{id}` - 更新指定食品項目
- **DELETE** `/food/{id}` - 刪除指定食品項目

### 📋 訂閱管理 API
- **GET** `/subscription` - 獲取所有訂閱項目
- **GET** `/subscription/{id}` - 獲取指定訂閱項目
- **POST** `/subscription` - 創建新訂閱項目
- **POST** `/subscription/{id}` - 更新指定訂閱項目
- **DELETE** `/subscription/{id}` - 刪除指定訂閱項目

## 資料模型

### FoodItem (食品項目)

**後端支援的完整欄位：**
```json
{
  "id": "1",
  "name": "食品名稱",
  "amount": 5,
  "to_date": "2024-01-15T00:00:00Z",
  "photo": "https://example.com/image.jpg",
  "photohash": null,
  "price": null,
  "shop": null
}
```

**支援的 CRUD 操作：**
- ✅ 讀取 (GET)
- ✅ 創建 (POST)
- ✅ 更新 (POST)
- ✅ 刪除 (DELETE)

**欄位對應：**
- `name` → FoodItem.Name
- `amount` → FoodItem.Amount
- `to_date` → FoodItem.ToDate
- `photo` → FoodItem.Photo
- `photohash` → FoodItem.PhotoHash
- `price` → FoodItem.Price
- `shop` → FoodItem.Shop

### Subscription (訂閱項目)

**後端支援的核心欄位：**
```json
{
  "id": "1",
  "name": "Netflix",
  "price": 290,
  "site": "https://netflix.com",
  "note": "影音串流服務"
}
```

**支援的 CRUD 操作：**
- ✅ 讀取 (GET) - 包含完整欄位
- ✅ 創建 (POST) - 僅支援基本欄位
- ✅ 更新 (POST) - 僅支援基本欄位
- ✅ 刪除 (DELETE)

**欄位對應：**
- `name` → Subscription.Name
- `price` → Subscription.Amount
- `site` → Subscription.Url
- `note` → Subscription.Description

**僅讀取時可用的欄位：**
- `nextdate` → Subscription.NextPaymentDate (讀取時可用，寫入時不支援)
- `account` → Subscription.Account (讀取時可用，寫入時不支援)

**僅本地儲存的欄位（不同步到後端）：**
- `category`: 分類（娛樂、工作等）
- `is_active`: 是否啟用
- `payment_method`: 付款方式
- `billing_cycle`: 計費週期
- `start_date`: 開始日期

## 使用方式

### 1. 設定 API 連接
1. 開啟應用程式
2. 點擊「系統設定」
3. 在 nhost 連接設定中確認：
   - Subdomain: `uxgwdiuehabbzenwtcqo`
   - Admin Secret: `cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`
4. 點擊「測試連接」驗證 API 可用性
5. 點擊「重新連接 API」重新初始化 API 服務

### 2. 使用食品管理
1. 點擊「食品管理」
2. 點擊「🔄 重新整理」從 API 載入最新資料
3. 點擊「+ 新增食品」創建新項目
4. 使用「編輯」和「刪除」按鈕管理現有項目

### 3. 使用訂閱管理
1. 點擊「訂閱管理」
2. 點擊「🔄 重新整理」從 API 載入最新資料
3. 點擊「+ 新增訂閱」創建新項目
4. 使用「編輯」和「刪除」按鈕管理現有項目

## 狀態指示器

### 載入狀態
- 🔄 旋轉圖示：表示正在執行 API 操作
- 按鈕禁用：防止在載入期間進行其他操作

### 狀態訊息
- 「資料載入成功」：成功從 API 載入資料
- 「從本地載入資料」：API 不可用，使用本地資料
- 「食品新增成功」：成功創建食品項目
- 「API 服務已重新連接」：重新初始化 API 服務

## 錯誤處理
- **網路錯誤**：自動切換到本地模式
- **認證錯誤**：顯示錯誤訊息，建議檢查 Admin Secret
- **伺服器錯誤**：顯示詳細錯誤訊息

## 資料同步
- **即時同步**：所有 CRUD 操作立即同步到後端
- **本地備份**：同時保存到本地檔案作為備份
- **衝突處理**：API 資料優先於本地資料

## 安全性
- 使用 HTTPS 加密傳輸
- Admin Secret 用於 API 認證
- 敏感資料在 UI 中以密碼形式顯示

## 疑難排解

### API 連接失敗
1. 檢查網路連接
2. 驗證 Subdomain 和 Admin Secret 是否正確
3. 點擊「測試連接」確認 API 狀態
4. 點擊「重新連接 API」重新初始化

### 資料不同步
1. 點擊「🔄 重新整理」手動同步
2. 檢查 API 連接狀態
3. 查看狀態訊息了解詳細情況

### 操作失敗
1. 檢查網路連接
2. 確認 API 服務正常運作
3. 查看控制台輸出了解詳細錯誤訊息