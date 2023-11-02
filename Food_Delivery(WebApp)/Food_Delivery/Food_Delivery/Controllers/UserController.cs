using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Food_Delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
                    if (stat != null && (stat == 3 || stat == access))
                        canVisit = user.Additionalid != null; //true;

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

        public async Task<Order?> createIfNoneTable(int additionalId)
        {
            Order? ord = await _foodDeliveryContext.Orders.FirstOrDefaultAsync(u => -u.IdOrders == additionalId);
            if (ord == null)
            {
                ord = new()
                {
                    IdOrders = -additionalId,
                    IdCustomerFk = additionalId,
                    TimeOrdered = DateTime.Now,
                    Totalcost = 0
                };
                await _foodDeliveryContext.Orders.AddAsync(ord);
                await _foodDeliveryContext.SaveChangesAsync();
            }
            return ord;
        }

        public async Task<ActionResult> Index()
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null? (int)ViewData["status"] : 0]);
            List<Dish> ds = await _foodDeliveryContext.Dishes.ToListAsync();
            List<DishOrderList> ordered_dishes = await _foodDeliveryContext.DishOrderLists.Where(d => -d.IdOrdersFk == (int)ViewData["additionalId"]).ToListAsync();
            Dictionary<int, int> dish_quantity = new();
            foreach (var dish in ordered_dishes)
                dish_quantity.Add(dish.IdDishFk, dish.Quantity);
            ViewData["dish_quantity"] = dish_quantity;
            return View(ds);
        }

        [HttpPost]
        public async Task<ActionResult> Add(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            bool ds = await _foodDeliveryContext.Dishes.AnyAsync(d => d.IdDish == id);
            if (ds)
            {
                Order? ord = await createIfNoneTable((int)ViewData["additionalId"]);
                DishOrderList? nw = await _foodDeliveryContext.DishOrderLists.FirstOrDefaultAsync(d=> d.IdOrdersFk == ord.IdOrders && d.IdDishFk == id);
                if (nw == null)
                {
                    nw = new()
                    {
                        IdDishOrderList = default,
                        IdDishFk = id,
                        IdOrdersFk = ord.IdOrders,
                        Quantity = 1
                    };
                    await _foodDeliveryContext.AddAsync(nw);
                }
                else
                    nw.Quantity += 1;
                await _foodDeliveryContext.SaveChangesAsync();
            }
            
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<ActionResult> Sub(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            bool ds = await _foodDeliveryContext.Dishes.AnyAsync(d => d.IdDish == id);
            if (ds)
            {
                Order? ord = await createIfNoneTable((int)ViewData["additionalId"]);
                DishOrderList? nw = await _foodDeliveryContext.DishOrderLists.FirstOrDefaultAsync(d => d.IdOrdersFk == ord.IdOrders && d.IdDishFk == id);
                if (nw != null)
                {
                    if(nw.Quantity > 1)
                        nw.Quantity -= 1;
                    else
                        _foodDeliveryContext.Remove(nw);
                }
                    
                await _foodDeliveryContext.SaveChangesAsync();
            }

            return RedirectToAction("Index", "User");
        }

        public ActionResult MyOrder(int id)
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdOrders > 0 && x.IdCustomer == id).ToList();
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
        public async Task<ActionResult> Basket()
        {
            var can = user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            Order ord = await createIfNoneTable((int)ViewData["additionalId"]);
            ord.DishOrderLists = _foodDeliveryContext.DishOrderLists.Where(d => d.IdOrdersFk == ord.IdOrders).ToList();
            return View(ord);
        }
    }
}
