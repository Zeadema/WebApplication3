using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class MyBasePageModel : PageModel
{
    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var storedSessionToken = context.HttpContext.Session.GetString("SessionToken");
        var currentRequestToken = context.HttpContext.Session.GetString("SessionToken");

        // Check if the session token exists and matches the stored token
        if (string.IsNullOrEmpty(currentRequestToken) || currentRequestToken != storedSessionToken)
        {
            // Token mismatch, perform logout or other actions
            context.Result = RedirectToPage("/Logout");
        }
    }
}
