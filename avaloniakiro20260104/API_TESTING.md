# GraphQL REST API 測試指南

## 概述
此文件提供完整的 Hasura GraphQL REST API 測試方法，包括手動測試和自動化測試。

## API 端點資訊
- **基礎 URL**: `https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest`
- **認證**: `x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr`

## 1. 使用 curl 進行手動測試

### 食品管理 API 測試

#### 1.1 獲取所有食品項目
```bash
curl -X GET \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/food" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Accept: application/json"
```

#### 1.2 創建新食品項目
```bash
curl -X POST \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/food" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "測試食品",
    "amount": 2,
    "to_date": "2026-01-15T00:00:00",
    "photo": "https://example.com/test.jpg",
    "price": 100,
    "shop": "測試商店"
  }'
```

#### 1.3 獲取指定食品項目
```bash
curl -X GET \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/food/{id}" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Accept: application/json"
```

#### 1.4 更新食品項目
```bash
curl -X POST \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/food/{id}" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "更新的食品",
    "amount": 3,
    "to_date": "2026-01-20T00:00:00",
    "photo": "https://example.com/updated.jpg",
    "price": 150,
    "shop": "新商店"
  }'
```

#### 1.5 刪除食品項目
```bash
curl -X DELETE \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/food/{id}" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr"
```

### 訂閱管理 API 測試

#### 2.1 獲取所有訂閱項目
```bash
curl -X GET \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/subscription" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Accept: application/json"
```

#### 2.2 創建新訂閱項目
```bash
curl -X POST \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/subscription" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "測試訂閱",
    "price": 299,
    "site": "https://test.com",
    "note": "測試用訂閱服務"
  }'
```

#### 2.3 獲取指定訂閱項目
```bash
curl -X GET \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/subscription/{id}" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Accept: application/json"
```

#### 2.4 更新訂閱項目
```bash
curl -X POST \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/subscription/{id}" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "更新的訂閱",
    "price": 399,
    "site": "https://updated.com",
    "note": "更新後的訂閱服務"
  }'
```

#### 2.5 刪除訂閱項目
```bash
curl -X DELETE \
  "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/subscription/{id}" \
  -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr"
```

## 2. 使用 Postman 測試

### 環境變數設定
```json
{
  "baseUrl": "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest",
  "adminSecret": "cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr"
}
```

### 預設標頭
```
x-hasura-admin-secret: {{adminSecret}}
Content-Type: application/json
Accept: application/json
```

## 3. 應用程式內測試

### 3.1 測試連接
在應用程式中：
1. 開啟「系統設定」
2. 點擊「測試連接」
3. 檢查連接狀態

### 3.2 測試 CRUD 操作
1. **食品管理**：
   - 點擊「🔄 重新整理」載入資料
   - 新增、編輯、刪除食品項目
   - 檢查控制台輸出

2. **訂閱管理**：
   - 點擊「🔄 重新整理」載入資料
   - 新增、編輯、刪除訂閱項目
   - 檢查控制台輸出

## 4. 常見回應格式

### 成功回應
```json
{
  "data": {
    "food": [
      {
        "id": "1",
        "name": "食品名稱",
        "amount": 2,
        "to_date": "2026-01-15T00:00:00Z",
        "photo": "https://example.com/image.jpg",
        "price": 100,
        "shop": "商店名稱"
      }
    ]
  }
}
```

### 錯誤回應
```json
{
  "error": "Unexpected variable photohash",
  "path": "$",
  "code": "bad-request"
}
```

## 5. 疑難排解

### 常見錯誤
1. **"Unexpected variable"**: 檢查請求中是否包含不支援的欄位
2. **401 Unauthorized**: 檢查 admin secret 是否正確
3. **404 Not Found**: 檢查端點 URL 是否正確
4. **500 Internal Server Error**: 檢查資料格式是否符合 GraphQL schema

### 除錯技巧
1. 檢查控制台輸出中的詳細錯誤訊息
2. 使用最小化的請求資料進行測試
3. 先測試 GET 請求確認連接正常
4. 逐步增加請求欄位找出問題所在

## 6. 效能測試

### 基本效能指標
- **回應時間**: < 500ms
- **成功率**: > 99%
- **併發處理**: 支援多個同時請求

### 測試腳本範例
```bash
#!/bin/bash
# 簡單的效能測試腳本
for i in {1..10}; do
  echo "Test $i"
  time curl -s -X GET \
    "https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest/food" \
    -H "x-hasura-admin-secret: cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr" \
    > /dev/null
done
```

## 7. 自動化測試建議

### 單元測試
- 測試 ApiService 中的每個方法
- 模擬 HTTP 回應進行測試
- 驗證錯誤處理邏輯

### 整合測試
- 測試完整的 CRUD 流程
- 驗證資料一致性
- 測試網路錯誤情況

### 端到端測試
- 測試完整的使用者工作流程
- 驗證 UI 與 API 的整合
- 測試離線/線上模式切換