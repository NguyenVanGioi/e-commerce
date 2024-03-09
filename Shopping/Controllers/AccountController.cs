using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.ViewModels;

namespace Shopping.Controllers
{
	public class AccountController : Controller
	{
		private RoleManager<IdentityRole> _roleManager;

		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;

		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_roleManager = roleManager;

		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(UserModel user)
		{
			if (ModelState.IsValid)
			{
				if (user.Password != user.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", "Mật khẩu và xác nhận mật khẩu không khớp");
					return View(user);
				}
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

				if (result.Succeeded)
				{
					// Tạo Role nếu chưa tồn tại
					if (!await _roleManager.RoleExistsAsync("User"))
					{
						await _roleManager.CreateAsync(new IdentityRole("User"));
					}

					// Lấy UserId của User vừa được thêm vào
					//string userId = newUser.Id;

					// Thêm User vào Role "User"
					await _userManager.AddToRoleAsync(newUser, "User");

					TempData["message"] = "Đăng ký tài khoản thành công";

					return Redirect("/account/login");
				}

				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        TempData["message"] = "Đăng nhập thành công";

                       

                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            // Nếu là Admin, chuyển hướng đến trang Admin/Index
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        else
                        {
                            // Nếu là User, chuyển hướng đến trang Home/Index
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            }
            return View(loginVM);
        }


        /*public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
           

            return Redirect(returnUrl);
        }*/
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (returnUrl == null)
            {
                TempData.Clear();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            TempData.Clear(); 

            return Redirect(returnUrl);
        }


    }
}
