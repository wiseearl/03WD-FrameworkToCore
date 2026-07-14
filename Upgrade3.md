# Framework Web Forms 升級到 ASP.NET Core MVC + VB.NET 紀錄（Core3）

## 1. 原始專案盤點

- 來源專案位於 `Framework/WebApplication1`。
- 專案型態為 ASP.NET Web Forms，使用 VB 與 .NET Framework 4.8。
- 啟動點在 `Global.asax.vb`，只註冊 FriendlyUrls 與 BundleConfig。
- `Default.aspx`、`About.aspx`、`Contact.aspx` 的 code-behind 幾乎沒有業務邏輯，主要是靜態內容頁。
- `Site.Master` 提供共用版型與導覽列。

## 2. 本次與前兩次升級的差異

| 項目 | Core（第一次） | Core2（第二次） | Core3（本次） |
| --- | --- | --- | --- |
| 目標架構 | ASP.NET Core MVC | ASP.NET Core Razor Pages | ASP.NET Core MVC |
| 語言 | C# | C# | **VB.NET** |
| 頁面結構 | Controller + View | PageModel + `.cshtml` | **VB Controller** + View |
| 路由方式 | MapControllerRoute | 約定式檔案路由 | MapControllerRoute |
| 啟動入口 | `Program.cs`（top-level） | `Program.cs`（top-level） | **`Program.vb`（Module/Sub Main）** |
| 頁面程式碼 | `HomeController.cs` | 各頁面的 PageModel | **`HomeController.vb`**（VB） |

## 3. 升級策略

- ASP.NET Core 不支援直接沿用 Web Forms 執行模型。
- 本次採用 **MVC + VB.NET** 作為遷移目標：
  - 示範原本 VB 開發者可繼續在 ASP.NET Core 中使用 VB.NET。
  - Controller 以 VB.NET 撰寫，保留 VB 語法習慣（`Inherits`、`Property`、`ReadOnly Property`）。
  - Razor Views（`.cshtml`）本身使用 Razor 語法，不受語言影響。
- VB.NET 不支援 C# 的 **top-level statements**，改以 `Module Program` + `Sub Main` 作為啟動入口。
- VB.NET 沒有 `ImplicitUsings`，改以 `.vbproj` 的 `<Import Namespace>` 項目實現全域命名空間匯入。

## 4. 建立新專案

本次以手動方式建立 SDK-style VB.NET Web 專案（`dotnet new` 目前沒有 VB MVC 範本）：

1. 建立 `Core3/WebApplication1.Core3/WebApplication1.Core3.vbproj`（`Microsoft.NET.Sdk.Web`，`net8.0`）。
2. 加入 `<Import Namespace>` 全域匯入，取代 C# 的 `ImplicitUsings`。
3. 建立 `Program.vb` 以 `Module Program / Sub Main` 架構啟動。

## 5. 版型遷移（Site.Master → _Layout.cshtml）

- 將 `Site.Master` 改寫為 `Views/Shared/_Layout.cshtml`。
- 導覽列改用 Razor Tag Helpers（`asp-controller`、`asp-action`）取代 Web Forms `runat="server"` href。
- 保留原本三個入口：首頁（`/`）、關於（`/About`）、連絡人（`/Contact`）。
- Footer 以 Razor 語法輸出年份（`@DateTime.Now.Year`），取代 Web Forms `<%: DateTime.Now.Year %>`。
- 移除 Web Forms 專屬元素：
  - `<form runat="server">` wrapper
  - `<asp:ScriptManager>`
  - `<asp:PlaceHolder>`
  - `<webopt:bundlereference>`

## 6. 頁面遷移（.aspx → MVC View）

| Web Forms 頁面 | MVC 對應 | URL |
| --- | --- | --- |
| `Default.aspx` + `Default.aspx.vb` | `Controllers/HomeController.vb` + `Views/Home/Index.cshtml` | `/` |
| `About.aspx` + `About.aspx.vb` | `Controllers/HomeController.vb` + `Views/Home/About.cshtml` | `/About` |
| `Contact.aspx` + `Contact.aspx.vb` | `Controllers/HomeController.vb` + `Views/Home/Contact.cshtml` | `/Contact` |

每個 Action Method（`Index`、`About`、`Contact`）以 `HttpGet` Attribute 指定路由，取代原本的 `Page_Load`。

## 7. 啟動設定遷移（Global.asax → Program.vb）

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

**遷移後 `Program.vb`：**

```vb
Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        builder.Services.AddControllersWithViews()

        Dim app = builder.Build()

        If Not app.Environment.IsDevelopment() Then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts()
            app.UseHttpsRedirection()
        End If

        app.UseStaticFiles()
        app.UseRouting()
        app.UseAuthorization()

        app.MapControllerRoute(
            name:="default",
            pattern:="{controller=Home}/{action=Index}/{id?}")

        app.Run()
    End Sub
End Module
```

## 8. VB.NET 語法差異對照（相對於 C# Core）

| C#（Core） | VB.NET（Core3） |
| --- | --- |
| `public class HomeController : Controller` | `Public Class HomeController` + `Inherits Controller` |
| `public IActionResult Index()` | `Public Function Index() As IActionResult` |
| `public string? RequestId { get; set; }` | `Public Property RequestId As String` |
| `public bool ShowRequestId => ...` | `Public ReadOnly Property ShowRequestId As Boolean` |
| `top-level statements（Program.cs）` | `Module Program` + `Sub Main` |
| `ImplicitUsings` | `<Import Namespace>` 於 `.vbproj` |
| `Error()` | `[Error]()` (VB 保留字需加中括號) |
| `new { controller = "Home" }` | `New With {.controller = "Home"}` |
| `Activity.Current?.Id ?? HttpContext.TraceIdentifier` | `If(Activity.Current?.Id, HttpContext.TraceIdentifier)` |

## 9. 舊技術對新技術映射

| Web Forms / .NET Framework | ASP.NET Core MVC（VB.NET）對應 |
| --- | --- |
| `Global.asax` | `Program.vb`（Module/Sub Main） |
| `RouteConfig.vb` + FriendlyUrls | `MapControllerRoute`（於 `Program.vb`） |
| `Site.Master` | `Views/Shared/_Layout.cshtml` |
| `.aspx` 頁面 | Razor View（`.cshtml`） |
| `.aspx.vb` code-behind（Page_Load） | `HomeController.vb`（Action Method） |
| `asp:ContentPlaceHolder` | `@RenderBody()` |
| `asp:Content` 內容區塊 | `.cshtml` 頁面主體 |
| `packages.config` | SDK-style + `PackageReference` |
| `System.Web` | `Microsoft.AspNetCore.*` |
| `<%: ... %>` 輸出運算式 | `@...` Razor 運算式 |

## 10. 專案結構對比

```text
Framework/WebApplication1/           →    Core3/WebApplication1.Core3/
├── Site.Master                           ├── Views/Shared/_Layout.cshtml
├── Default.aspx                          ├── Views/Home/Index.cshtml
├── Default.aspx.vb                       ├── Controllers/HomeController.vb  (VB)
├── About.aspx                            ├── Views/Home/About.cshtml
├── About.aspx.vb                         ├── Controllers/HomeController.vb  (VB)
├── Contact.aspx                          ├── Views/Home/Contact.cshtml
├── Contact.aspx.vb                       ├── Controllers/HomeController.vb  (VB)
├── Global.asax / RouteConfig.vb          ├── Program.vb                     (VB)
├── Web.config                            ├── appsettings.json
└── packages.config                       └── WebApplication1.Core3.vbproj
```

## 11. 本次未直接搬移的項目

- `ViewSwitcher.ascx`：舊式行動/桌面切換機制，在響應式 Bootstrap 版面下不再需要。
- `BundleConfig.vb`：Web Forms 捆綁設定，改用 ASP.NET Core 靜態資源機制（`UseStaticFiles`）。
- `Web Forms ScriptManager`：JavaScript 由 `wwwroot/lib/` 下的靜態 CDN 套件取代。
- `Site.Mobile.Master`：行動版專屬 Master Page，Bootstrap 響應式設計已涵蓋行動裝置。

## 12. 驗證方式

在根目錄執行：

```powershell
& "C:\Program Files\dotnet\dotnet.exe" build .\Core3\WebApplication1.Core3\WebApplication1.Core3.vbproj
& "C:\Program Files\dotnet\dotnet.exe" run --project .\Core3\WebApplication1.Core3\WebApplication1.Core3.vbproj
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

## 13. 後續建議

若要進一步從「範例遷移」擴展到正式專案，可依序處理：

1. 將 Web Forms server controls（如 `GridView`、`FormView`）改寫為 Razor Views + Tag Helpers。
2. 將表單提交從 `Button_Click` 事件改寫為 Controller 的 `[HttpPost]` Action Method（VB 的 `<HttpPost()>`）。
3. 將 `Web.config` 中的連線字串與設定搬移到 `appsettings.json` 與環境變數。
4. 補上 xUnit 整合測試（`WebApplicationFactory(Of Program)`，VB 泛型語法）。
5. 視需要加入 DI 服務（Repository、Service Layer）以建構函式注入取代原本 code-behind 直接操作資料的模式。
