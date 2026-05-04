using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Core2.Pages;

public class AboutModel : PageModel
{
    public void OnGet()
    {
        // 對應原本 About.aspx.vb 的 Page_Load
        // 原 code-behind 沒有業務邏輯，PageModel 僅作路由入口
    }
}
