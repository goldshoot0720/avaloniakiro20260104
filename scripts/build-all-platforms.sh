#!/bin/bash

# 鋒兄Next資訊管理系統 - 多平台建置腳本
# 此腳本會為所有支援的平台建置應用程式

set -e

echo "🚀 開始建置鋒兄Next資訊管理系統..."

# 清理之前的建置
echo "🧹 清理之前的建置..."
rm -rf publish/
mkdir -p publish

# 建置配置
PROJECT_PATH="avaloniakiro20260104/avaloniakiro20260104.csproj"
CONFIGURATION="Release"

# 支援的平台
declare -a PLATFORMS=(
    "win-x64"
    "win-arm64" 
    "osx-x64"
    "osx-arm64"
    "linux-x64"
    "linux-arm64"
)

# 為每個平台建置
for platform in "${PLATFORMS[@]}"
do
    echo "📦 建置 $platform..."
    dotnet publish $PROJECT_PATH \
        -c $CONFIGURATION \
        -r $platform \
        --self-contained \
        -o "publish/$platform" \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true \
        --verbosity quiet
    
    echo "✅ $platform 建置完成"
done

# 創建壓縮檔
echo "📁 創建發布壓縮檔..."
cd publish

for platform in "${PLATFORMS[@]}"
do
    if [[ $platform == win-* ]]; then
        # Windows 平台使用 zip
        zip -r "${platform}.zip" "$platform/" > /dev/null
        echo "📦 已創建 ${platform}.zip"
    else
        # Unix 平台使用 tar.gz
        tar -czf "${platform}.tar.gz" "$platform/"
        echo "📦 已創建 ${platform}.tar.gz"
    fi
done

cd ..

echo ""
echo "🎉 所有平台建置完成！"
echo "📂 發布檔案位於 publish/ 目錄"
echo ""
echo "支援的平台："
for platform in "${PLATFORMS[@]}"
do
    echo "  - $platform"
done
echo ""
echo "🚀 準備發布到 GitHub Releases！"