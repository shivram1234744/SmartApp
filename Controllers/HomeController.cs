using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbManager _db;

        public HomeController(DbManager db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Index(LoginModel model)
        //    {
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    using (SqlConnection con = new SqlConnection(_connStr))
        //    {
        //        string query = "SELECT RoleId, Email, Password, role_mst.Role as Role FROM User_mst  left join role_mst on role_mst.RoleId=User_mst.Role WHERE Email=@Email AND User_mst.Role=@Role";

        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@Email", model.Email);
        //        cmd.Parameters.AddWithValue("@Role", model.RoleId);

        //        con.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            string dbPassword = reader["Password"].ToString();


        //            if (model.Password == dbPassword)
        //            {
        //                var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name, reader["Email"].ToString()),
        //                new Claim(ClaimTypes.Role, reader["Role"].ToString()),

        //            };

        //                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        //                var principal = new ClaimsPrincipal(identity);

        //                await HttpContext.SignInAsync("MyCookieAuth", principal);

        //                return RedirectToAction("Dashboard", "Admin");
        //            }
        //        }
        //    }

        //    ModelState.AddModelError("", "Invalid email or password");
        //    return View(model);
        //}


        [HttpPost]
        public async Task<JsonResult> Login(LoginModel model)
        {
            var user = _db.User_mst.FirstOrDefault(x =>
                x.Email.ToLower() == model.Email.ToLower() &&
                x.Password == model.Password);

            if (user != null)
            {
                // 🎯 Role mapping (3 roles)
                string roleName = user.Role == 1 ? "Admin"
                                 : user.Role == 2 ? "Client"
                                 : user.Role == 3 ? "Executor"
                                 : "Unknown";

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, roleName)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Json(new { success = true, role = roleName });
            }

            return Json(new { success = false });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
