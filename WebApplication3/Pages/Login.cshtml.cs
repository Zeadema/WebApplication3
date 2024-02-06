using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApplication3.Model;
using WebApplication3.ViewModels;


namespace WebApplication3.Pages
{
	[ValidateReCaptcha]
	public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IDataProtectionProvider dataProtectionProvider;
		public LoginModel(SignInManager<ApplicationUser> signInManager,
					 IDataProtectionProvider dataProtectionProvider)
        {
            this.signInManager = signInManager;
			this.dataProtectionProvider = dataProtectionProvider;
		}
        public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostAsync()
        {

            if (ModelState.IsValid)
			{
				var protector = dataProtectionProvider.CreateProtector("CreditCardProtection");
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                LModel.RememberMe, true);
                if (identityResult.Succeeded)
                {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, "c@c.com"),
                        new Claim(ClaimTypes.Email, "c@c.com")
                    };
                    var i = new ClaimsIdentity(claims, "Cookie");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    await HttpContext.SignInAsync("Cookie", claimsPrincipal);
                    // Generate a unique session token
                    var sessionToken = Guid.NewGuid().ToString();

                    // Store the session token in session
                    HttpContext.Session.SetString("SessionToken", sessionToken);
                        
                    return RedirectToPage("Index");
                }
                else if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", $"Account locked out. Please Try again Later");
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password incorrect");
                }
            }
            return Page();
        }
    }
}