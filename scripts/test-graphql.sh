#!/bin/bash

# GraphQL API 測試腳本
# 用於測試 Hasura GraphQL 端點

GRAPHQL_URL="https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/v1/graphql"
ADMIN_SECRET="cu#34&yjF3Cr%fgxB#WA,4r4^c=Igcwr"

echo "🚀 開始測試 GraphQL API"
echo "================================"

# 測試食品查詢
echo "📊 測試食品管理 GraphQL"
echo "-------------------"

echo "1. 獲取所有食品項目..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "query GetFoodItems { food { id name amount to_date photo price shop } }"
  }' | jq '.' || echo "❌ 獲取食品列表失敗"

echo -e "\n2. 創建新食品項目..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "mutation CreateFoodItem($object: food_insert_input!) { insert_food_one(object: $object) { id name amount to_date } }",
    "variables": {
      "object": {
        "name": "GraphQL測試食品",
        "amount": 1,
        "to_date": "2026-02-01T00:00:00",
        "photo": "https://example.com/graphql-test.jpg",
        "price": 75,
        "shop": "GraphQL測試商店"
      }
    }
  }' | jq '.' || echo "❌ 創建食品項目失敗"

echo -e "\n📋 測試訂閱管理 GraphQL"
echo "-------------------"

echo "1. 獲取所有訂閱項目..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "query GetSubscriptions { subscription { id name nextdate price site note account } }"
  }' | jq '.' || echo "❌ 獲取訂閱列表失敗"

echo -e "\n2. 創建新訂閱項目..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "mutation CreateSubscription($object: subscription_insert_input!) { insert_subscription_one(object: $object) { id name nextdate price site note } }",
    "variables": {
      "object": {
        "name": "GraphQL測試訂閱",
        "nextdate": "2026-02-01T00:00:00",
        "price": 299,
        "site": "https://graphql-test.com",
        "note": "GraphQL測試用訂閱服務",
        "account": "test@graphql.com"
      }
    }
  }' | jq '.' || echo "❌ 創建訂閱項目失敗"

echo -e "\n🔍 測試進階 GraphQL 功能"
echo "-------------------"

echo "1. 條件查詢 - 獲取價格大於100的食品..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "query GetExpensiveFood { food(where: {price: {_gt: 100}}) { id name price shop } }"
  }' | jq '.' || echo "❌ 條件查詢失敗"

echo -e "\n2. 分頁查詢 - 獲取前5個訂閱..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "query GetSubscriptionsWithLimit { subscription(limit: 5, order_by: {name: asc}) { id name price } }"
  }' | jq '.' || echo "❌ 分頁查詢失敗"

echo -e "\n3. 聚合查詢 - 計算訂閱總數和平均價格..."
curl -s -X POST \
  "$GRAPHQL_URL" \
  -H "x-hasura-admin-secret: $ADMIN_SECRET" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "query GetSubscriptionStats { subscription_aggregate { aggregate { count avg { price } sum { price } } } }"
  }' | jq '.' || echo "❌ 聚合查詢失敗"

echo -e "\n🎉 GraphQL API 測試完成"
echo "================================"

echo -e "\n💡 GraphQL 優勢展示："
echo "- ✅ 靈活的欄位選擇"
echo "- ✅ 強大的條件查詢"
echo "- ✅ 內建分頁和排序"
echo "- ✅ 聚合查詢功能"
echo "- ✅ 單一端點處理所有操作"