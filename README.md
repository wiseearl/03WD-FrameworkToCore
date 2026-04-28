# 03WD-FrameworkToCore

此倉庫示範將 ASP.NET Web Forms 專案從 .NET Framework 升級到 ASP.NET Core 的做法。

來源專案位於 `Framework/WebApplication1`，為 VB + ASP.NET Web Forms + .NET Framework 4.8。

升級後的目標專案位於 `Core/WebApplication1.Core`，為 ASP.NET Core MVC + .NET 8。

## 專案目的

- 保留原 Web Forms 範例專案作為對照。
- 在 `Core` 中重建一個可執行的 ASP.NET Core Web 專案。
- 將原本的首頁、關於、連絡人頁面與共用版型遷移到 ASP.NET Core MVC。
- 以文件方式逐步記錄升級過程與技術映射。

## 目錄結構

```text
03WD-FrameworkToCore/
|- Framework/
|  |- WebApplication1/        # 原始 ASP.NET Web Forms (VB, .NET Framework 4.8)
|- Core/
|  |- WebApplication1.Core/   # 升級後 ASP.NET Core MVC (.NET 8)
|- Upgrade.md                 # 升級步驟與技術映射紀錄
```

## 升級結果

本次升級採用重建式遷移，而不是直接轉換原專案檔。

原因如下：

- ASP.NET Core 不支援 Web Forms 執行模型。
- ASP.NET Core Web 專案沒有直接對應的 VB Web Forms 遷移路徑。
- 原範例專案頁面邏輯非常少，適合改寫成 MVC Controller + Razor Views。

已完成的對應如下：

- `Default.aspx` -> `Views/Home/Index.cshtml`
- `About.aspx` -> `Views/Home/About.cshtml`
- `Contact.aspx` -> `Views/Home/Contact.cshtml`
- `Site.Master` -> `Views/Shared/_Layout.cshtml`
- `Global.asax` / `RouteConfig.vb` -> `Program.cs` + MVC Routing

## 執行方式

需求：

- 已安裝 .NET 8 SDK 或更新版本

建置：

```powershell
dotnet build .\Core\WebApplication1.Core\WebApplication1.Core.csproj
```

執行：

```powershell
dotnet run --project .\Core\WebApplication1.Core\WebApplication1.Core.csproj
```

啟動後可檢查：

- `/`
- `/About`
- `/Contact`

## 文件索引

- [Upgrade.md](Upgrade.md): 完整升級步驟、技術映射、未移植項目與後續建議。

## 補充說明

- 原始 `Framework` 專案仍保留在倉庫中，方便比對升級前後差異。
- `ViewSwitcher.ascx`、Web Forms ScriptManager、BundleConfig 等舊技術沒有直接搬移，改以 ASP.NET Core 的路由、Layout 與靜態資源機制取代。
- 目前 `Core/WebApplication1.Core` 為最小可運作版本，適合作為後續正式遷移的起點。