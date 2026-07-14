Imports System.Diagnostics
Imports Microsoft.AspNetCore.Mvc
Imports WebApplication1.Core3.Models

Namespace Controllers

    ''' <summary>
    ''' 首頁控制器，對應原 Web Forms 的 Default.aspx、About.aspx、Contact.aspx。
    ''' 原本的 Page_Load 事件改由各 Action Method（Index、About、Contact）取代。
    ''' </summary>
    Public Class HomeController
        Inherits Controller

        ''' <summary>
        ''' 首頁（對應原 Default.aspx）
        ''' </summary>
        <HttpGet("")>
        Public Function Index() As IActionResult
            Return View()
        End Function

        ''' <summary>
        ''' 關於頁（對應原 About.aspx）
        ''' </summary>
        <HttpGet("About")>
        Public Function About() As IActionResult
            Return View()
        End Function

        ''' <summary>
        ''' 連絡人頁（對應原 Contact.aspx）
        ''' </summary>
        <HttpGet("Contact")>
        Public Function Contact() As IActionResult
            Return View()
        End Function

        ''' <summary>
        ''' 錯誤頁
        ''' </summary>
        <ResponseCache(Duration:=0, Location:=ResponseCacheLocation.None, NoStore:=True)>
        Public Function [Error]() As IActionResult
            Return View(New ErrorViewModel With {
                .RequestId = If(Activity.Current?.Id, HttpContext.TraceIdentifier)
            })
        End Function

    End Class

End Namespace
