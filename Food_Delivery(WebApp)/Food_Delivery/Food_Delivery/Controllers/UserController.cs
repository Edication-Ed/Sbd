using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery.Controllers
{
    public class UserController : Controller
    {
        public const string cookie_loggeduser_id = "cookie_loggeduser_id";
        public const string cookie_loggeduser_passcode = "cookie_loggeduser_key";
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public UserController(FoodDeliveryContext foodDelivery)
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
            List<Dish> ds = _foodDeliveryContext.Dishes.ToList();
            return View(ds);
        }
        public ActionResult MyOrder(int id)
        {
            user_init();
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdCustomer == id).ToList();
            return View(ds);
        }
        public ActionResult Chek(int id)
        {
            user_init();
            List<DishOrderListView> Dr = _foodDeliveryContext.DishOrderListViews.Where(x => x.IdOrdersFk == id).ToList();
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdOrders == id).ToList();
            ViewData["Total"] = ds.First().Totalcost;
            return View(Dr);
        }
    }
}
