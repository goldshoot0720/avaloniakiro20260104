@echo off
REM 鋒兄Next資訊管理系統 - Windows 多平台建置腳本

echo 🚀 開始建置鋒兄Next資訊管理系統...

REM 清理之前的建置
echo 🧹 清理之前的建置...
if exist publish rmdir /s /q publish
mkdir publish

REM 建置配置
set PROJECT_PATH=avaloniakiro20260104/avaloniakiro20260104.csproj
set CONFIGURATION=Release

REM 建置 Windows x64
echo 📦 建置 win-x64...
dotnet publish %PROJECT_PATH% -c %CONFIGURATION% -r win-x64 --self-contained -o "publish/win-x64" -p:PublishSingleFile=true -p:PublishTrimmed=true --verbosity quiet
echo ✅ win-x64 建置完成

REM 建置 Windows ARM64
echo 📦 建置 win-arm64...
dotnet publish %PROJECT_PATH% -c %CONFIGURATION% -r win-arm64 --self-contained -o "publish/win-arm64" -p:PublishSingleFile=true -p:PublishTrimmed=true --verbosity quiet
echo ✅ win-arm64 建置完成

REM 建置 macOS x64
echo 📦 建置 osx-x64...
dotnet publish %PROJECT_PATH% -c %CONFIGURATION% -r osx-x64 --self-contained -o "publish/osx-x64" -p:PublishSingleFile=true -p:PublishTrimmed=true --verbosity quiet
echo ✅ osx-x64 建置完成

REM 建置 macOS ARM64
echo 📦 建置 osx-arm64...
dotnet publish %PROJECT_PATH% -c %CONFIGURATION% -r osx-arm64 --self-contained -o "publish/osx-arm64" -p:PublishSingleFile=true -p:PublishTrimmed=true --verbosity quiet
echo ✅ osx-arm64 建置完成

REM 建置 Linux x64
echo 📦 建置 linux-x64...
dotnet publish %PROJECT_PATH% -c %CONFIGURATION% -r linux-x64 --self-contained -o "publish/linux-x64" -p:PublishSingleFile=true -p:PublishTrimmed=true --verbosity quiet
echo ✅ linux-x64 建置完成

REM 建置 Linux ARM64
echo 📦 建置 linux-arm64...
dotnet publish %PROJECT_PATH% -c %CONFIGURATION% -r linux-arm64 --self-contained -o "publish/linux-arm64" -p:PublishSingleFile=true -p:PublishTrimmed=true --verbosity quiet
echo ✅ linux-arm64 建置完成

echo.
echo 🎉 所有平台建置完成！
echo 📂 發布檔案位於 publish\ 目錄
echo.
echo 支援的平台：
echo   - win-x64
echo   - win-arm64
echo   - osx-x64
echo   - osx-arm64
echo   - linux-x64
echo   - linux-arm64
echo.
echo 🚀 準備發布到 GitHub Releases！

pause