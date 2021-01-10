using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Web.Dashboard.Controllers
{
    public class AccountController : BaseController
    {
        private readonly JobDbContext _dbContext;

        public AccountController(JobDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string redirect)
        {
            if (redirect.NullOrEmpty())
            {
                redirect = "/home/index";
            }

            return View(new UserViewModel()
            { 
                Redirect = redirect
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Name == model.UserName);

            if (model.UserName.NullOrEmpty())
            {
                return Ok("用户名不能为空".Error());
            }

            if (model.Password.NullOrEmpty())
            {
                return Ok("密码不能为空".Error());
            }

            if (user.HashPassword != model.Password.MD5Encrypt())
            {
                return Ok("密码不正确".Error());
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("display_name", user.DisplayName.ToString()),
            };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "CoreJobCookie")));
            return Ok(EmptyJson.Success());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserViewModel model)
        {
            var user = await _dbContext.User.FindAsync(GetCurrentUserId());

            if (user == null)
            {
                return Ok("用户不存在".Error());
            }

            if (model.Password.NullOrEmpty())
            {
                return Ok("密码不能为空".Error());
            }

            if (user.HashPassword != model.Password.MD5Encrypt())
            {
                return Ok("密码不正确".Error());
            }

            var newHashPassword = model.NewPassword.MD5Encrypt();
            if (user.HashPassword == newHashPassword)
            {
                return Ok("新密码不能与旧密码一致".Error());
            }

            user.HashPassword = newHashPassword;

            _dbContext.Entry(user).Property(nameof(DashboardUser.HashPassword)).IsModified = true;

            await _dbContext.SaveChangesAsync();

            //            await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"Update {nameof(DashboardUser)} 
            //set {nameof(DashboardUser.HashPassword)}={newHashPassword} Where Id={user.Id}");

            return Ok(EmptyJson.Success());
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            var dbUser = await _dbContext.User.FirstOrDefaultAsync(x => x.Name == model.UserName);

            if (dbUser != null)
            {
                return Ok("用户已存在".Error());
            }

            var newUser = new DashboardUser()
            {
                DisplayName = model.UserDisplayName,
                Disabled = false,
                HashPassword = model.Password.MD5Encrypt(),
                InTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Name = model.UserName,
            };

            await _dbContext.User.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return Ok(EmptyJson.Success());
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CoreJobCookie");
            return RedirectToAction(nameof(AccountController.Login));
        }

        public IActionResult Index() => View();

        public IActionResult Password() => View();

        public async Task<IActionResult> AccountList(AccountListViewModel model)
        {
            var query = _dbContext.User.AsQueryable();

            var count = await query.CountAsync().ConfigureAwait(false);
            var items = await query.Skip((model.PageNum - 1) * model.PageSize)
                                    .Take(model.PageSize)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

            return Ok(new
            {
                total = count,
                rows = items.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name
                })
            }.Success());
        }

        public IActionResult New()
        {
            return View("Edit", new UserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            _dbContext.User.Remove(new  DashboardUser() { Id = id });
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
            }.Success());
        }
    }
}
