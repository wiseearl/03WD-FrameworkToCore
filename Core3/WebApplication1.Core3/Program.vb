' Program.vb
' VB.NET 不支援 C# 的 top-level statements，
' 改以 Module Program + Sub Main 方式撰寫啟動入口。

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' 加入 MVC Controller + View 服務
        ' AddRazorRuntimeCompilation：VB.NET 專案無法在建置時自動編譯 .cshtml，
        ' 需在執行階段從磁碟載入 Razor View 檔案。
        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation()

        Dim app = builder.Build()

        ' 設定 HTTP 請求管線
        If Not app.Environment.IsDevelopment() Then
            app.UseExceptionHandler("/Home/Error")
            ' 預設 HSTS 為 30 天，正式環境請依需求調整。
            app.UseHsts()
            app.UseHttpsRedirection()
        End If

        app.UseStaticFiles()
        app.UseRouting()
        app.UseAuthorization()

        ' 預設路由：{controller=Home}/{action=Index}/{id?}
        app.MapControllerRoute(
            name:="default",
            pattern:="{controller=Home}/{action=Index}/{id?}")

        ' 另支援 /About、/Contact 短路由對應到 HomeController
        app.MapControllerRoute(
            name:="pages",
            pattern:="{action=Index}",
            defaults:=New With {.controller = "Home"})

        app.Run()
    End Sub
End Module
