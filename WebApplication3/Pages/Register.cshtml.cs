using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IDataProtectionProvider dataProtectionProvider;


        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                     IDataProtectionProvider dataProtectionProvider)
        {
            this.userManager = userManager;
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
                var existingUser = await userManager.FindByEmailAsync(RModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("RModel.Email", "Email address is already in use.");
                    return Page();
                }

                var protector = dataProtectionProvider.CreateProtector("CreditCardProtection");

				var user = new ApplicationUser
				{
					UserName = RModel.Email,
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					MobileNumber = RModel.MobileNumber,
					CreditCardNumber = protector.Protect(RModel.CreditCardNumber),
					BillingAddress = protector.Protect(RModel.BillingAddress),
					ShippingAddress = protector.Protect(RModel.ShippingAddress),
					Email = RModel.Email,
					Password = protector.Protect(RModel.Password),
					PhotoFileName = RModel.Photo
				};

				
				var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {

					await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return Page();
        }
    }
}
