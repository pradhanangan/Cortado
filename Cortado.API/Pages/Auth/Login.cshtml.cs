using Amazon.CognitoIdentityProvider.Model;
using Customers.Application.Customers;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Common.Authentication;
using System.Security.Claims;

namespace Cortado.API.Pages.Auth
{
    public class LoginModel : PageModel
    {

        private readonly IUserManagement _userManagement;
        private readonly ITokenParser _tokenParser;
        private readonly ISender _mediatr;

        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();

        public string ErrorMessage { get; set; }

        [BindNever]
        public string ReturnUrl { get; set; } = "/";

        public LoginModel(IUserManagement userManagement, ITokenParser tokenParser, ISender mediatr)
        {
            
            _userManagement = userManagement;
            _tokenParser = tokenParser;
            _mediatr = mediatr;
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
            ErrorMessage = string.Empty;
            // Set cache control headers to prevent caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        public async Task<IActionResult> OnPostAsync([FromForm] string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            var ret = await UseCognitoForAuth();
            if (ret)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToPage("/Index");
            }

            // Simulated user login logic
            //if (Input.Username == "admin" && Input.Password == "password")
            //{
            //    var claims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, Input.Username),
            //        new Claim(ClaimTypes.Role, "Admin")
            //    };

            //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //    var principal = new ClaimsPrincipal(identity);

            //    // Save cookies/
            //    await HttpContext.SignInAsync("MyCookieAuth", principal);

            //    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //    {

            //        return LocalRedirect(returnUrl);
            //    }

            //    return RedirectToPage("/Admin/Index");
            //}
            //ErrorMessage = "Invalid username or password.";

            return Page();
        }

        private async Task<bool> UseCognitoForAuth()
        {
            
            

            try
            {
                
                var response = await _userManagement.SignInAsync(Input.Username, Input.Password);
                
                var idToken = response.IdToken;
                var accessToken = response.AccessToken;


                var sub = _tokenParser.GetSubFromIdToken(idToken);
                    var customer = await _mediatr.Send(new GetCustomerByIdentityIdQuery(Guid.Parse(sub)));
                    // Sign in with Cookie Auth
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Input.Username),
                        new Claim(ClaimTypes.Email, Input.Username),

                        new Claim("sub", sub),
                        new Claim("IdToken", idToken),
                        new Claim("AccessToken", accessToken)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                //await HttpContext.SignInAsync("MyCookieAuth", principal);

                    return true;
                //}

            }
            catch (NotAuthorizedException)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return false;
            }
        }
    }
    public class LoginInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
