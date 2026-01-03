# 鋒兄Next資訊管理系統

一個使用 Avalonia UI 開發的跨平台桌面資訊管理應用程式。

## 功能特色

### 📊 儀表板
- 統計資訊總覽
- 快速數據展示
- 即時狀態監控

### 🍎 食品管理
- 食品庫存管理
- 到期日期追蹤
- 狀態分類（正常/即將過期/已過期）
- 新增、編輯食品資訊

### 📋 訂閱管理
- 訂閱服務追蹤
- 費用統計
- 付款日期提醒
- 月費總計計算

### 🎬 影片介紹
- 影片內容展示
- 支援本地和線上影片
- 漸層背景設計
- 播放時長顯示

### 🎵 鋒兄音樂歌詞
- 音樂歌詞管理
- 歌曲收藏功能

### ℹ️ 關於我們
- 公司資訊展示
- 聯絡方式
- 版本資訊

## 技術規格

- **框架**: .NET 8.0
- **UI 框架**: Avalonia UI 11.3.10
- **架構模式**: MVVM (Model-View-ViewModel)
- **相依套件**: CommunityToolkit.Mvvm 8.2.1
- **平台支援**: Windows, macOS, Linux

## 系統需求

- .NET 8.0 Runtime (自包含發布版本無需額外安裝)
- 支援的作業系統：
  - Windows 10/11
  - macOS 10.15+
  - Linux (各主要發行版)

## 建置說明

### 開發環境建置
```bash
# 複製專案
git clone https://github.com/goldshoot0720/avaloniakiro20260104.git
cd avaloniakiro20260104

# 還原套件
dotnet restore

# 建置專案
dotnet build

# 執行測試
dotnet test

# 執行應用程式
dotnet run --project avaloniakiro20260104
```

### 快速多平台建置
我們提供了便利的建置腳本來一次建置所有支援的平台：

**Linux/macOS:**
```bash
./scripts/build-all-platforms.sh
```

**Windows:**
```cmd
scripts\build-all-platforms.bat
```

### 手動發布應用程式
```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Windows ARM64
dotnet publish -c Release -r win-arm64 --self-contained -p:PublishSingleFile=true

# macOS x64
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true

# Linux ARM64
dotnet publish -c Release -r linux-arm64 --self-contained -p:PublishSingleFile=true
```

## 專案結構

```
avaloniakiro20260104/
├── .github/
│   └── workflows/
│       └── build-and-release.yml    # GitHub Actions CI/CD
├── avaloniakiro20260104/            # 主應用程式
│   ├── Assets/                      # 資源檔案
│   ├── Models/                      # 資料模型
│   │   ├── FoodItem.cs             # 食品項目模型
│   │   ├── Subscription.cs         # 訂閱模型
│   │   └── VideoContent.cs         # 影片內容模型
│   ├── ViewModels/                  # 視圖模型
│   │   ├── MainWindowViewModel.cs  # 主視窗視圖模型
│   │   ├── StringEqualsConverter.cs # 字串比較轉換器
│   │   └── ViewModelBase.cs        # 視圖模型基底類別
│   ├── Views/                       # 視圖
│   │   ├── MainWindow.axaml        # 主視窗 XAML
│   │   └── MainWindow.axaml.cs     # 主視窗程式碼
│   ├── App.axaml                   # 應用程式 XAML
│   ├── App.axaml.cs                # 應用程式程式碼
│   ├── Program.cs                  # 程式進入點
│   └── ViewLocator.cs              # 視圖定位器
├── avaloniakiro20260104.Tests/      # 測試專案
│   └── UnitTest1.cs                # 單元測試
├── scripts/                         # 建置腳本
│   ├── build-all-platforms.sh      # Linux/macOS 建置腳本
│   └── build-all-platforms.bat     # Windows 建置腳本
├── CHANGELOG.md                     # 更新日誌
├── LICENSE                          # MIT 授權條款
└── README.md                        # 專案說明
```

## 功能截圖

應用程式包含現代化的 UI 設計，具有：
- 藍色主題側邊選單
- 響應式卡片佈局
- 漸層背景效果
- 清晰的資料展示

## 開發團隊

**鋒兄途哥公開資訊**
- 專業的公開關係服務
- 致力於管理解決方案
- 創新技術與專業服務

## 版本資訊

- **版本**: v1.0.0
- **技術棧**: Next.js + TypeScript
- **開發地區**: Made with ❤️ in Taiwan

## 授權條款

本專案採用 MIT 授權條款。

## 聯絡資訊

如有任何問題或建議，歡迎聯絡我們：
- 📧 Email: contact@fengtuage.com
- 🌐 官方網站: www.fengtuage.com
- 📞 客服專線: +886-2-1234-5678

---

© 2025 鋒兄途哥公開資訊有限公司 版權所有
