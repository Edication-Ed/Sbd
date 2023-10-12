using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery.Controllers
{
    public class UserController : Controller
    {
        public const string cookie_loggeduser_id = "cookie_loggeduser_id";
        public const string cookie_loggeduser_passcode = "cookie_loggeduser_key";
        public enum statuses{ 
            admin = 0,
            user = 1,
            delivery_man = 2
        }
        public static String[] default_controller = { "Home", "User", "Cur" };
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public UserController(FoodDeliveryContext foodDelivery)
        {
            _foodDeliveryContext = foodDelivery;
        }

        bool user_init(int access = 1)
        {
            bool canVisit = false;
            if (CookieHave(cookie_loggeduser_id))
            {
                int id = int.Parse(GetFromCookie(cookie_loggeduser_id));
                string pass = GetFromCookie(cookie_loggeduser_passcode);
                Userlogin? user = _foodDeliveryContext.Userlogins.FirstOrDefault(x => x.Id == id && x.Passcode == pass);
                if (user != null)
                {
                    ViewData["userData"] = user.Username;
                    ViewData["additionalId"] = user.Additionalid;
                    int stat = (int)user.Status;
                    ViewData["status"] = stat;
                    if (stat != null && ( stat == 3 || stat == access))
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
            if (!can)
                return RedirectToAction("Index", default_controller[ViewData["status"] != null? (int)ViewData["status"] : 0]);
            List<Dish> ds = _foodDeliveryContext.Dishes.ToList();
            return View(ds);
        }
        public ActionResult MyOrder(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdCustomer == id).ToList();
            return View(ds);
        }
        public ActionResult Chek(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            List<DishOrderListView> Dr = _foodDeliveryContext.DishOrderListViews.Where(x => x.IdOrdersFk == id).ToList();
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdOrders == id).ToList();
            ViewData["Total"] = ds.First().Totalcost;
            return View(Dr);
        }
    }
}
