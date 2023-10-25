using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Food_Delivery.Models;

namespace Food_Delivery.Controllers
{
    public class CurController : Controller
    {
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public CurController(FoodDeliveryContext foodDelivery)
        {
            _foodDeliveryContext = foodDelivery;
        }

        bool user_init(int access = 2)
        {
            bool canVisit = false;
            if (CookieHave(constants.cookie_loggeduser_id))
            {
                int id = int.Parse(GetFromCookie(constants.cookie_loggeduser_id));
                string pass = GetFromCookie(constants.cookie_loggeduser_passcode);
                Userlogin? user = _foodDeliveryContext.Userlogins.FirstOrDefault(x => x.Id == id && x.Passcode == pass);
                if (user != null)
                {
                    ViewData["userData"] = user.Username;
                    ViewData["additionalId"] = user.Additionalid;
                    int stat = (int)user.Status;
                    ViewData["status"] = stat;
                    if (stat != null && (stat == 3 || stat == access))
                        canVisit = true;

                }
                else
                {
                    ViewData["userData"] = "";
                }
            }
            else
            {
                ViewData["userData"] = "";
            }
            return canVisit;
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
            var can = user_init();
            if (!can) return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            List<Deliverylist> dell = _foodDeliveryContext.Deliverylists.ToList();
            List<String> adres = new List<String>();
            List<decimal> cost = new List<decimal>();
            List<String> pay = new List<String>();
            foreach (Deliverylist del in dell) {
                Order? ord = _foodDeliveryContext.Orders.FirstOrDefault(o => o.IdOrders == del.IdOrdersFk);
                Customer? cus = _foodDeliveryContext.Customers.FirstOrDefault(c => c.IdCustomer == ord.IdCustomerFk);
                if (cus != null)
                    adres.Add(string.Format("г. {0}, улица {1}, дом {2}{4}, квартира {3}", cus.City, cus.Street, cus.HouseNumber, cus.Apartment, cus.Building));
                else
                    adres.Add("");

                if (ord != null)
                    cost.Add(ord.Totalcost);
                else
                    cost.Add(0);
            }
            ViewData["Add"] = adres;
            ViewData["Cost"] = cost;
            return View(dell);
        }
    }
}
