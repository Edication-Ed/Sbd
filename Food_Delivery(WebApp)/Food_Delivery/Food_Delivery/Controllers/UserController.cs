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

        async Task<bool> user_init(int access = 1)
        {
            bool canVisit = false;
            if (CookieHave(constants.cookie_loggeduser_id))
            {
                string id = GetFromCookie(constants.cookie_loggeduser_id);
                string pass = GetFromCookie(constants.cookie_loggeduser_passcode);
                Userlogin? user = await constants.getUserById(id, pass);
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
                    CookieRemove(constants.cookie_loggeduser_id);
                    CookieRemove(constants.cookie_loggeduser_passcode);
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

        void CookieRemove(string name)
        {
            HttpContext.Response.Cookies.Delete(name);
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
            var can = await user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null? (int)ViewData["status"] : 0]);
            List<Dish> ds = await _foodDeliveryContext.Dishes.ToListAsync();
            List<DishOrderList> ordered_dishes = await _foodDeliveryContext.DishOrderLists.Where(d => -d.IdOrdersFk == (int)ViewData["additionalId"]).ToListAsync();
            Dictionary<int, int> dish_quantity = new();
            foreach (var dish in ordered_dishes)
            {
                try
                {
                    _ = dish_quantity[dish.IdDishFk];
                    _foodDeliveryContext.Remove(dish);
                    await _foodDeliveryContext.SaveChangesAsync();
                }
                catch
                {
                    dish_quantity.Add(dish.IdDishFk, dish.Quantity);
                }
            }
            ViewData["dish_quantity"] = dish_quantity;
            return View(ds);
        }

        [HttpPost]
        public async Task<ActionResult> Add(int id)
        {
            var can = await user_init();
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
            var can = await user_init();
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

        async public Task<ActionResult> MyOrder()
        {
            var can = await user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            int id = (int)ViewData["additionalId"];
            List<OrdersView> ds = _foodDeliveryContext.OrdersViews.Where(x => x.IdOrders > 0 && x.IdCustomer == id).ToList();
            return View(ds);
        }
        async public Task<ActionResult> Chek(int id)
        {
            var can = await user_init();
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
            var can = await user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            Order ord = await createIfNoneTable((int)ViewData["additionalId"]);
            ord.DishOrderLists = _foodDeliveryContext.DishOrderLists.Where(d => d.IdOrdersFk == ord.IdOrders).ToList();

            List<Dish> ds = await _foodDeliveryContext.Dishes.ToListAsync();
            Dictionary<int, Dish> dishes = new();
            foreach (var dish in ds)
                dishes.Add(dish.IdDish, dish);
            List<Dish> dishs = new();
            int count = 0;
            foreach (var dish in ord.DishOrderLists)
            {
                dishes[dish.IdDishFk].DishOrderLists.Add(dish);
                dishs.Add(dishes[dish.IdDishFk]);
                count += dish.Quantity;
            }
            ViewData["dish_quantity"] = dishs;
            ViewData["count"] = count;
            return View(ord);
        }
        [HttpPost]
        public async Task<ActionResult> Zak()
        {
            var can = await user_init();
            if (!can)
                return RedirectToAction("Index", constants.default_controller[ViewData["status"] != null ? (int)ViewData["status"] : 0]);
            int additionalId = (int)ViewData["additionalId"];
            Order ord = await createIfNoneTable(additionalId);
            if (ord.Totalcost == 0)
                return RedirectToAction("Basket");
            ord = new()
            {
                IdOrders = default,
                IdCustomerFk = additionalId,
                TimeOrdered = DateTime.Now,
                Totalcost = 0
            };
            await _foodDeliveryContext.Orders.AddAsync(ord);
            await _foodDeliveryContext.SaveChangesAsync();
            foreach (var dish in await _foodDeliveryContext.DishOrderLists
                .Where(d=> d.IdOrdersFk == -additionalId).ToListAsync())
            {
                dish.IdOrdersFk = ord.IdOrders;
            }
            await _foodDeliveryContext.SaveChangesAsync();
            return RedirectToAction("MyOrder");
        }
    }
}
