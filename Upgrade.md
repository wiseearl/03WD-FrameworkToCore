# Framework Web Forms 升級到 ASP.NET Core 紀錄

## 1. 原始專案盤點

- 來源專案位於 `Framework/WebApplication1`。
- 專案型態為 ASP.NET Web Forms，使用 VB 與 .NET Framework 4.8。
- 啟動點在 `Global.asax.vb`，只註冊 FriendlyUrls 與 BundleConfig。
- `Default.aspx`、`About.aspx`、`Contact.aspx` 的 code-behind 幾乎沒有業務邏輯，主要是靜態內容頁。
- `Site.Master` 提供共用版型與導覽列。

## 2. 升級策略

- ASP.NET Core 不支援直接沿用 Web Forms 執行模型。
- ASP.NET Core 也不提供 Web Forms 型式的 VB Web 專案遷移路徑。
- 因此本次升級採用 **重建式遷移**：
  - 在 `Core` 建立新的 ASP.NET Core MVC 專案。
  - 將 Web Forms 的頁面內容與導覽結構，重新映射為 MVC Controller + Razor Views。
  - 保留原有頁面語意與 URL：`/`、`/About`、`/Contact`。

## 3. 建立新專案

在 `Core` 目錄執行：

```powershell
dotnet new mvc -n WebApplication1.Core -f net8.0
```

結果：

- 產生新的 ASP.NET Core MVC 專案 `Core/WebApplication1.Core`。
- 使用 SDK-style 專案檔與 `PackageReference`。
- 不再需要 `packages.config`、`Global.asax`、`System.Web`、Web Forms ScriptManager 與 bundling 設定。

## 4. 路由與啟動調整

- 使用 `Program.cs` 取代 `Global.asax` 作為應用程式啟動入口。
- 保留 MVC 預設路由，並讓 `HomeController` 提供下列頁面：
  - `Index()` 對應首頁
  - `About()` 對應關於頁
  - `Contact()` 對應連絡人頁
- 透過 Attribute Routing 與 Controller Route，讓 URL 可直接使用 `/About` 與 `/Contact`。

## 5. UI 與版型遷移

- 將 `Site.Master` 改寫為 `Views/Shared/_Layout.cshtml`。
- 導覽列保留原本三個入口：首頁、關於、連絡人。
- Footer 以 Razor 語法輸出年份，取代 Web Forms `<%: DateTime.Now.Year %>`。
- `Default.aspx` 內容改寫到 `Views/Home/Index.cshtml`。
- `About.aspx` 改寫到 `Views/Home/About.cshtml`。
- `Contact.aspx` 改寫到 `Views/Home/Contact.cshtml`。

## 6. 舊技術對新技術映射

| Web Forms / .NET Framework | ASP.NET Core 對應 |
| --- | --- |
| `Global.asax` | `Program.cs` |
| `RouteConfig.vb` + FriendlyUrls | Endpoint Routing / Attribute Routing |
| `Site.Master` | `_Layout.cshtml` |
| `.aspx` 頁面 | Razor View |
| code-behind `Page_Load` | Controller Action / ViewModel / Middleware |
| `packages.config` | SDK-style + `PackageReference` |
| `System.Web` | `Microsoft.AspNetCore.*` |

## 7. 本次未直接搬移的項目

- `ViewSwitcher.ascx` 為舊式行動/桌面切換機制，在響應式 Bootstrap 版面下通常不再需要，因此未移植。
- `BundleConfig.vb` 與 Web Forms ScriptManager 腳本註冊未移植，改用 ASP.NET Core 樣板內建靜態資源管理方式。
- Web Forms 頁面生命週期事件 (`Page_Load`) 沒有實際業務邏輯，因此未另外轉寫。

## 8. 驗證方式

在 `Core/WebApplication1.Core` 執行：

```powershell
dotnet build
dotnet run
```

檢查項目：

- `/` 可正常顯示首頁
- `/About` 可正常顯示關於頁
- `/Contact` 可正常顯示連絡人頁

## 9. 後續建議

若要進一步從「範例遷移」擴展到正式專案，可依序處理：

1. 將 Web Forms server controls 改寫為 Razor Tag Helpers 或前端元件。
2. 將事件驅動 code-behind 改寫為 Controller + Service + ViewModel。
3. 將 `Web.config` 中的設定搬移到 `appsettings.json` 與環境變數。
4. 補上整合測試與部署設定。