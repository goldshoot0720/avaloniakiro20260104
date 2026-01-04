@echo off
REM GraphQL REST API 測試腳本 (Windows 版本)
REM 用於測試 Hasura REST 端點

set BASE_URL=https://uxgwdiuehabbzenwtcqo.hasura.eu-central-1.nhost.run/api/rest
set ADMIN_SECRET=cu#34^&yjF3Cr%%fgxB#WA,4r4^^c=Igcwr

echo 🚀 開始測試 GraphQL REST API
echo ================================

echo 📊 測試食品管理 API
echo -------------------

echo 1. 獲取所有食品項目...
curl -s -X GET "%BASE_URL%/food" -H "x-hasura-admin-secret: %ADMIN_SECRET%" -H "Accept: application/json"
if %errorlevel% neq 0 echo ❌ 獲取食品列表失敗

echo.
echo 2. 創建新食品項目...
curl -s -X POST "%BASE_URL%/food" ^
  -H "x-hasura-admin-secret: %ADMIN_SECRET%" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\": \"API測試食品\", \"amount\": 1, \"to_date\": \"2026-02-01T00:00:00\", \"photo\": \"https://example.com/test.jpg\", \"price\": 50, \"shop\": \"測試商店\"}"

echo.
echo 📋 測試訂閱管理 API
echo -------------------

echo 1. 獲取所有訂閱項目...
curl -s -X GET "%BASE_URL%/subscription" -H "x-hasura-admin-secret: %ADMIN_SECRET%" -H "Accept: application/json"
if %errorlevel% neq 0 echo ❌ 獲取訂閱列表失敗

echo.
echo 2. 創建新訂閱項目...
curl -s -X POST "%BASE_URL%/subscription" ^
  -H "x-hasura-admin-secret: %ADMIN_SECRET%" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\": \"API測試訂閱\", \"price\": 199, \"site\": \"https://api-test.com\", \"note\": \"API測試用訂閱服務\"}"

echo.
echo 🎉 API 測試完成
echo ================================
pause