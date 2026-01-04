#!/bin/bash

# GraphQL REST API 測試腳本
# 用於測試 Hasura REST 端點

BASE_URL="https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest"
ADMIN_SECRET="cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr"

echo "🚀 開始測試 GraphQL REST API"
echo "================================"

# 測試食品 API
echo "📊 測試食品管理 API"
echo "-------------------"

echo "1. 獲取所有食品項目..."
curl -s -X GET \
  "$BASE_URL/food" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Accept: application/json" | jq '.' || echo "❌ 獲取食品列表失敗"

echo -e "\n2. 創建新食品項目..."
FOOD_RESPONSE=$(curl -s -X POST \
  "$BASE_URL/food" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "API測試食品",
    "amount": 1,
    "to_date": "2026-02-01T00:00:00",
    "photo": "https://example.com/test.jpg",
    "price": 50,
    "shop": "測試商店"
  }')

echo "$FOOD_RESPONSE" | jq '.' || echo "❌ 創建食品項目失敗: $FOOD_RESPONSE"

# 嘗試從回應中提取 ID
FOOD_ID=$(echo "$FOOD_RESPONSE" | jq -r '.id // .food.id // empty' 2>/dev/null)

if [ ! -z "$FOOD_ID" ] && [ "$FOOD_ID" != "null" ]; then
  echo -e "\n3. 獲取指定食品項目 (ID: $FOOD_ID)..."
  curl -s -X GET \
    "$BASE_URL/food/$FOOD_ID" \
    -H "x-hasura-admin-secret: $ADMIN_SECRET" \
    -H "Accept: application/json" | jq '.' || echo "❌ 獲取指定食品項目失敗"

  echo -e "\n4. 更新食品項目..."
  curl -s -X POST \
    "$BASE_URL/food/$FOOD_ID" \
    -H "x-hasura-admin-secret: $ADMIN_SECRET" \
    -H "Content-Type: application/json" \
    -d '{
      "name": "更新的API測試食品",
      "amount": 2,
      "to_date": "2026-02-15T00:00:00",
      "photo": "https://example.com/updated.jpg",
      "price": 75,
      "shop": "更新的測試商店"
    }' | jq '.' || echo "❌ 更新食品項目失敗"

  echo -e "\n5. 刪除食品項目..."
  DELETE_RESPONSE=$(curl -s -X DELETE \
    "$BASE_URL/food/$FOOD_ID" \
    -H "x-hasura-admin-secret: $ADMIN_SECRET")
  
  if [ -z "$DELETE_RESPONSE" ] || echo "$DELETE_RESPONSE" | jq -e '.error' >/dev/null 2>&1; then
    echo "❌ 刪除食品項目失敗: $DELETE_RESPONSE"
  else
    echo "✅ 食品項目刪除成功"
  fi
else
  echo "⚠️  無法獲取食品項目 ID，跳過後續測試"
fi

echo -e "\n📋 測試訂閱管理 API"
echo "-------------------"

echo "1. 獲取所有訂閱項目..."
curl -s -X GET \
  "$BASE_URL/subscription" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Accept: application/json" | jq '.' || echo "❌ 獲取訂閱列表失敗"

echo -e "\n2. 創建新訂閱項目..."
SUB_RESPONSE=$(curl -s -X POST \
  "$BASE_URL/subscription" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "API測試訂閱",
    "price": 199,
    "site": "https://api-test.com",
    "note": "API測試用訂閱服務"
  }')

echo "$SUB_RESPONSE" | jq '.' || echo "❌ 創建訂閱項目失敗: $SUB_RESPONSE"

# 嘗試從回應中提取 ID
SUB_ID=$(echo "$SUB_RESPONSE" | jq -r '.id // .subscription.id // empty' 2>/dev/null)

if [ ! -z "$SUB_ID" ] && [ "$SUB_ID" != "null" ]; then
  echo -e "\n3. 獲取指定訂閱項目 (ID: $SUB_ID)..."
  curl -s -X GET \
    "$BASE_URL/subscription/$SUB_ID" \
    -H "x-hasura-admin-secret: $ADMIN_SECRET" \
    -H "Accept: application/json" | jq '.' || echo "❌ 獲取指定訂閱項目失敗"

  echo -e "\n4. 更新訂閱項目..."
  curl -s -X POST \
    "$BASE_URL/subscription/$SUB_ID" \
    -H "x-hasura-admin-secret: $ADMIN_SECRET" \
    -H "Content-Type: application/json" \
    -d '{
      "name": "更新的API測試訂閱",
      "price": 299,
      "site": "https://updated-api-test.com",
      "note": "更新後的API測試訂閱服務"
    }' | jq '.' || echo "❌ 更新訂閱項目失敗"

  echo -e "\n5. 刪除訂閱項目..."
  DELETE_RESPONSE=$(curl -s -X DELETE \
    "$BASE_URL/subscription/$SUB_ID" \
    -H "x-hasura-admin-secret: $ADMIN_SECRET")
  
  if [ -z "$DELETE_RESPONSE" ] || echo "$DELETE_RESPONSE" | jq -e '.error' >/dev/null 2>&1; then
    echo "❌ 刪除訂閱項目失敗: $DELETE_RESPONSE"
  else
    echo "✅ 訂閱項目刪除成功"
  fi
else
  echo "⚠️  無法獲取訂閱項目 ID，跳過後續測試"
fi

echo -e "\n🎉 API 測試完成"
echo "================================"