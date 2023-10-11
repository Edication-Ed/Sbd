using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery.Controllers
{
    public class CurController : Controller
    {
        public const string cookie_loggeduser_id = "cookie_loggeduser_id";
        public const string cookie_loggeduser_passcode = "cookie_loggeduser_key";
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public CurController(FoodDeliveryContext foodDelivery)
        {
            _foodDeliveryContext = foodDelivery;
        }

        void user_init()
        {
            if (CookieHave(cookie_loggeduser_id))
            {
                int id = int.Parse(GetFromCookie(cookie_loggeduser_id));
                string pass = GetFromCookie(cookie_loggeduser_passcode);
                Userlogin? user = _foodDeliveryContext.Userlogins.FirstOrDefault(x => x.Id == id && x.Passcode == pass);
                if (user != null) { 
                    ViewData["userData"] = user.Username;
                    ViewData["additionalId"] = user.Additionalid;
                    ViewData["status"] = user.Status;
                }
                else
                    ViewData["userData"] = "";
            }
            else
            {
                ViewData["userData"] = "";
            }
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

        public ActionResult Index()
        {
            user_init();
            List<Deliverylist> dell = _foodDeliveryContext.Deliverylists.ToList();
            ViewData["Dell"] = dell;
            List<String> adres = new List<String>();
            foreach(Deliverylist del in dell) { 
                Customer? cus = _foodDeliveryContext.Customers.FirstOrDefault(c => c.IdCustomer == _foodDeliveryContext.Orders.FirstOrDefault(o=>o.IdOrders == del.IdOrdersFk).IdCustomerFk);
                if (cus != null)
                    adres.Add(string.Format("г. {0}, улица {1}, дом {2}{4}, квартира {3}", cus.City, cus.Street, cus.HouseNumber, cus.Apartment, cus.Building));
                else
                    adres.Add("");
            }
            ViewData["Add"] = adres;
            return View(dell);
        }
    }
}
