# Framework Web Forms 升級到 ASP.NET Core Razor Pages 紀錄（Core2）

## 1. 原始專案盤點

- 來源專案位於 `Framework/WebApplication1`。
- 專案型態為 ASP.NET Web Forms，使用 VB 與 .NET Framework 4.8。
- 啟動點在 `Global.asax.vb`，只註冊 FriendlyUrls 與 BundleConfig。
- `Default.aspx`、`About.aspx`、`Contact.aspx` 的 code-behind 幾乎沒有業務邏輯，主要是靜態內容頁。
- `Site.Master` 提供共用版型與導覽列。

## 2. 本次與 Core 升級的差異

| 項目 | Core（第一次） | Core2（第二次） |
| --- | --- | --- |
| 目標架構 | ASP.NET Core MVC | ASP.NET Core Razor Pages |
| 頁面結構 | Controller + View | PageModel + `.cshtml` |
| 路由方式 | Attribute Routing + `Program.cs` MapControllerRoute | 約定式檔案路由（`/About` → `Pages/About.cshtml`） |
| 啟動入口 | `Program.cs` + `AddControllersWithViews` | `Program.cs` + `AddRazorPages` |
| 頁面程式碼 | `HomeController.cs` | 各頁面的 `PageModel`（`.cshtml.cs`） |

## 3. 升級策略

- ASP.NET Core 不支援直接沿用 Web Forms 執行模型。
- 本次採用 **Razor Pages** 作為遷移目標（有別於 Core 的 MVC 方式）：
  - Razor Pages 模型更接近 Web Forms 的「一頁一檔」概念，每個 `.aspx` 頁面對應一個 `.cshtml` + `.cshtml.cs` 配對。
  - 不需要額外的 Controller，路由由資料夾結構（`Pages/` 目錄）決定。
  - 適合將原本 code-behind 業務邏輯逐步搬移為 PageModel 的 `OnGet` / `OnPost`。

## 4. 建立新專案

在根目錄執行：

```powershell
dotnet new webapp -n WebApplication1.Core2 -f net8.0 --output ".\Core2\WebApplication1.Core2"
```

結果：

- 產生新的 ASP.NET Core Razor Pages 專案 `Core2/WebApplication1.Core2`。
- 使用 SDK-style 專案檔（`Microsoft.NET.Sdk.Web`）與 `PackageReference`。
- 不再需要 `packages.config`、`Global.asax`、`System.Web`、Web Forms ScriptManager 與 bundling 設定。

## 5. 版型遷移（Site.Master → _Layout.cshtml）

- 將 `Site.Master` 改寫為 `Pages/Shared/_Layout.cshtml`。
- 導覽列改用 Razor Tag Helpers（`asp-page`）取代 Web Forms `runat="server"` href。
- 保留原本三個入口：首頁（`/`）、關於（`/About`）、連絡人（`/Contact`）。
- Footer 以 Razor 語法輸出年份（`@DateTime.Now.Year`），取代 Web Forms `<%: DateTime.Now.Year %>`。
- 移除 Web Forms 專屬元素：
  - `<form runat="server">` wrapper
  - `<asp:ScriptManager>`
  - `<asp:PlaceHolder>`
  - `<webopt:bundlereference>`

## 6. 頁面遷移（.aspx → Razor Pages）

| Web Forms 頁面 | Razor Pages 配對 | URL |
| --- | --- | --- |
| `Default.aspx` + `Default.aspx.vb` | `Pages/Index.cshtml` + `Pages/Index.cshtml.cs` | `/` |
| `About.aspx` + `About.aspx.vb` | `Pages/About.cshtml` + `Pages/About.cshtml.cs` | `/About` |
| `Contact.aspx` + `Contact.aspx.vb` | `Pages/Contact.cshtml` + `Pages/Contact.cshtml.cs` | `/Contact` |

每個 `.cshtml.cs` 繼承 `PageModel`，並以 `OnGet()` 取代原本的 `Page_Load`。

## 7. 啟動設定遷移（Global.asax → Program.cs）

**原始 `Global.asax.vb`：**

```vb
Imports System.Web.Optimization

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
    End Sub
End Class
```

**遷移後 `Program.cs`：**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
```

## 8. 舊技術對新技術映射

| Web Forms / .NET Framework | ASP.NET Core Razor Pages 對應 |
| --- | --- |
| `Global.asax` | `Program.cs` |
| `RouteConfig.vb` + FriendlyUrls | 約定式檔案路由（`Pages/` 目錄結構） |
| `Site.Master` | `Pages/Shared/_Layout.cshtml` |
| `.aspx` 頁面 | `.cshtml` Razor Page |
| `.aspx.vb` code-behind（Page_Load） | `.cshtml.cs` PageModel（OnGet / OnPost） |
| `asp:ContentPlaceHolder` | `@RenderBody()` |
| `asp:Content` 內容區塊 | `.cshtml` 頁面主體 |
| `packages.config` | SDK-style + `PackageReference` |
| `System.Web` | `Microsoft.AspNetCore.*` |
| `<%: ... %>` 輸出運算式 | `@...` Razor 運算式 |

## 9. 專案結構對比

```text
Framework/WebApplication1/           →    Core2/WebApplication1.Core2/
├── Site.Master                           ├── Pages/Shared/_Layout.cshtml
├── Default.aspx                          ├── Pages/Index.cshtml
├── Default.aspx.vb                       ├── Pages/Index.cshtml.cs
├── About.aspx                            ├── Pages/About.cshtml
├── About.aspx.vb                         ├── Pages/About.cshtml.cs
├── Contact.aspx                          ├── Pages/Contact.cshtml
├── Contact.aspx.vb                       ├── Pages/Contact.cshtml.cs
├── Global.asax / RouteConfig.vb          ├── Program.cs
└── Web.config                            └── appsettings.json
```

## 10. 本次未直接搬移的項目

- `ViewSwitcher.ascx`：舊式行動/桌面切換機制，在響應式 Bootstrap 版面下不再需要。
- `BundleConfig.vb`：Web Forms 捆綁設定，改用 ASP.NET Core 靜態資源機制（`UseStaticFiles`）。
- `Web Forms ScriptManager`：JavaScript 由 `wwwroot/lib/` 下的靜態 CDN 套件取代。
- `Site.Mobile.Master`：行動版專屬 Master Page，Bootstrap 響應式設計已涵蓋行動裝置。

## 11. 驗證方式

在 `Core2/WebApplication1.Core2` 執行：

```powershell
dotnet build .\Core2\WebApplication1.Core2\WebApplication1.Core2.csproj
dotnet run --project .\Core2\WebApplication1.Core2\WebApplication1.Core2.csproj
```

建置結果：

```
建置成功。
    0 個警告
    0 個錯誤
```

啟動後檢查項目：

- `/` 可正常顯示首頁
- `/About` 可正常顯示關於頁
- `/Contact` 可正常顯示連絡人頁

## 12. 後續建議

若要進一步從「範例遷移」擴展到正式專案，可依序處理：

1. 將 Web Forms server controls（如 `GridView`、`FormView`）改寫為 Razor Pages Tag Helpers 或前端元件。
2. 將表單提交從 `Button_Click` 事件改寫為 PageModel 的 `OnPost()` 方法。
3. 將 `Web.config` 中的連線字串與設定搬移到 `appsettings.json` 與環境變數。
4. 補上 xUnit 整合測試（`WebApplicationFactory<Program>`）。
5. 視需要加入 DI 服務（Repository、Service Layer）取代原本 code-behind 直接操作資料的模式。
