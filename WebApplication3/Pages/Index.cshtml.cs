using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
    [Authorize]
    public class IndexModel : MyBasePageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public string SessionKeyTime { get; private set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user != null)
                {
                    ViewData["FirstName"] = user.FirstName;
                    ViewData["LastName"] = user.LastName;
                    ViewData["CreditCardNumber"] = user.CreditCardNumber;
                    ViewData["MobileNumber"] = user.MobileNumber;
                    ViewData["BillingAddress"] = user.BillingAddress;
                    ViewData["ShippingAddress"] = user.ShippingAddress;
                    ViewData["Email"] = user.Email;
                    ViewData["Photo"] = user.PhotoFileName;
                }

            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            HttpContext.Session.Remove("SessionToken");

            await _signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }

    }
}