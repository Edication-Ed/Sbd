using Food_Delivery.Models;
using Auth_Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;

namespace Auth_Login.Controllers
{


    public class SignupFaillure
    {
        string _login;
        string _password;
        string _confirmation;
        string reason;

        

        public SignupFaillure(string a, string b, string c, string d)
        {
            _login = a;
            _password = b;
            _confirmation = c;
            reason = d;
        }

        public SignupFaillure()
        {
            _login = "";
            _password = "";
            _confirmation = "";
            reason = "";
        }

        public string Login()
        {
            return _login;
        }
        public string Password()
        {
            return _password;
        }
        public string Confirmation() {
            return _confirmation;
        }
        public string Reason()
        {
            return reason;
        }
    }

    public class HomeController : Controller
    {
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public HomeController(FoodDeliveryContext foodDelivery)
        {
            _foodDeliveryContext = foodDelivery;
        }
        void user_init()
        {
            if (CookieHave(constants.cookie_loggeduser_id))
            {
                int id = int.Parse(GetFromCookie(constants.cookie_loggeduser_id));
                string pass = GetFromCookie(constants.cookie_loggeduser_passcode);
                Userlogin? user = _foodDeliveryContext.Userlogins.FirstOrDefault(x => x.Id == id && x.Passcode == pass);
                if (user != null)
                    ViewData["userData"] = user.Username;
                else
                {
                    ViewData["userData"] = "";
                    CookieRemove(constants.cookie_loggeduser_id);
                    CookieRemove(constants.cookie_loggeduser_passcode);
                }
            } else
            {
                ViewData["userData"] = "";
            }
        }
        

        public IActionResult Check()
        {
            user_init();
            return View(_foodDeliveryContext.Userlogins.ToList());
        }

        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));

                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        void SaveToCookie(string name, string item, bool sessional)
        {
            CookieOptions options = new CookieOptions();
            if (!sessional)
                options.MaxAge = TimeSpan.FromDays(365);
                
            options.Secure = true;
            options.SameSite = SameSiteMode.Strict;
            if (Request.Cookies.TryGetValue(name, out _))
            {
                HttpContext.Response.Cookies.Delete(name);
            }
            HttpContext.Response.Cookies.Append(name, item, options);
        }

        string GetFromCookie(string name)
        {
            string item;
            if (!Request.Cookies.TryGetValue(name, out item))
                return "";
            return item;
        }

        bool CookieHave(string name)
        {
            return Request.Cookies.TryGetValue(name, out _);
        }

        void CookieRemove(string name)
        {
            HttpContext.Response.Cookies.Delete(name);
        }

        public IActionResult Index()
        {
            return Redirect("Login");
        }

        public async Task<IActionResult> Login(string username, string passcode, bool rememberme)
        {
            if (CookieHave(constants.cookie_loggeduser_id))
            {
                int id = int.Parse(GetFromCookie(constants.cookie_loggeduser_id));
                string pass = GetFromCookie(constants.cookie_loggeduser_passcode);
                var use = _foodDeliveryContext.Userlogins.FirstOrDefault(x => x.Id == id && x.Passcode == pass);
                if (use == null)
                    return View();
                switch (use.Status)
                {
                    case 3:
                        return RedirectToAction("Index", "Food");
                    case 2:
                        return RedirectToAction("Index", "Cur");
                    case 1:
                        return RedirectToAction("Index", "User");
                }
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passcode))
                return View();
            var user = await _foodDeliveryContext.Userlogins.FirstOrDefaultAsync(user => user.Username.Equals(username) && user.Passcode.Equals(ComputeSha256Hash(passcode)));
            if (user == null)
            {
                ViewData["Username"] = username;
                return View();
            }
            SaveToCookie(constants.cookie_loggeduser_id, user.Id.ToString(), !rememberme);
            SaveToCookie(constants.cookie_loggeduser_passcode, user.Passcode, !rememberme);
            switch (user.Status) {
                case 3:
                    return RedirectToAction("Index", "Food");
                case 2:
                    return RedirectToAction("Index", "Cur");
                case 1:
                    return RedirectToAction("Index", "User");
            }
            return View();
        }

        public async Task<IActionResult> signout(string url)
        {
            if (CookieHave(constants.cookie_loggeduser_id))
                CookieRemove(constants.cookie_loggeduser_id);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Signup(string username, string passcode, string confirm, bool curier = false)
        {
            if (CookieHave(constants.cookie_loggeduser_id))
                return RedirectToAction("Index", "Food");
            string reason = "";
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passcode))
                return View(new SignupFaillure());
            if (passcode != confirm)
                reason += "Password and password confirmation not the same!\n";
            var user = await _foodDeliveryContext.Userlogins.FirstOrDefaultAsync(user => user.Username.Equals(username));
            if (user != null)
                reason += "This username have been taken already!\n";
            
            if (reason != "")
                return View(new SignupFaillure(username, passcode, confirm, reason));
            user = new Userlogin()
            {
                Username = username,
                Passcode = ComputeSha256Hash(passcode),
                Status = 1 - (curier ? 1 : 0)
            };
            _foodDeliveryContext.Add(user);
            _foodDeliveryContext.SaveChanges();
            if (!curier) {
                await Login(username, passcode, false);
                return RedirectToAction("Additions");
            }
            return RedirectToAction("CurierInfo", new {user_id = user.Id });
        }

        public async Task<ActionResult> CurierInfo(int user_id)
        {
            Userlogin? st = await _foodDeliveryContext.Userlogins.FirstOrDefaultAsync(user => user.Id ==user_id);
            return View(st!=null? st.Status : 0);
        }

        public ActionResult Additions()
        {
            user_init();
            return View();

        }
        public ActionResult Privacy()
        {
            user_init();
            return View();
        }

        [HttpPost]
        public IActionResult Additions(string LN, string FN, string PA, string Tel, string GR, string UL, int DO, char ST, int KV, IFormFile image)
        {
            if (image != null)
            {
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Cus", image.FileName);
                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                int uid = int.Parse(GetFromCookie(constants.cookie_loggeduser_id));
                Userlogin user = _foodDeliveryContext.Userlogins.First(e => e.Id == uid);
                Customer cus = new Customer { CustomerLastname = LN, CustomerFirstname = FN, CustomerPatronymic = PA, CustomerPhonenumber = Tel, City = GR, Street = UL, HouseNumber = (short)DO, Building = ST, Apartment = (short)KV, Foto = image.FileName };
                _foodDeliveryContext.Add(cus);
                _foodDeliveryContext.SaveChanges();
                user.Additionalid = cus.IdCustomer;
                _foodDeliveryContext.SaveChanges();
            }
            return RedirectToAction("Cus", "Food");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}