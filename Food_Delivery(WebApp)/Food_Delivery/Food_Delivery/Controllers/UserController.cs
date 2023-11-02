using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Food_Delivery.Models;

namespace Food_Delivery.Controllers
{
    public class UserController : Controller
    {
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public UserController(FoodDeliveryContext foodDelivery)
        {
            _foodDeliveryContext = foodDelivery;
        }

        bool user_init(int access = 1)
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
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null? (int)ViewData["status"] : 0]);
            List<Dish> ds = _foodDeliveryContext.Dishes.ToList();
            return View(ds);
        }
        public ActionResult MyOrder(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdCustomer == id).ToList();
            return View(ds);
        }
        public ActionResult Chek(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            List<DishOrderListView> Dr = _foodDeliveryContext.DishOrderListViews.Where(x => x.IdOrdersFk == id).ToList();
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdOrders == id).ToList();
            ViewData["Total"] = ds.First().Totalcost;
            ViewData["Date"] = ds.First().TimeOrdered;
            ViewData["LastName"] = ds.First().CustomerLastname;
            ViewData["FirstName"] = ds.First().CustomerFirstname;
            ViewData["Patronomic"] = ds.First().CustomerPatronymic;
            return View(Dr);
        }
        public ActionResult Basket()
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            return View();
        }
    }
}
