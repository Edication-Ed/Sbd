using Auth_Login.Controllers;
using Food_Delivery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;

namespace Food_Delivery.Controllers
{
    public class FoodController : Controller
    {
        public const string cookie_loggeduser_id = "cookie_loggeduser_id";
        public const string cookie_loggeduser_passcode = "cookie_loggeduser_key";
        private readonly FoodDeliveryContext _foodDeliveryContext;
        public FoodController(FoodDeliveryContext foodDelivery)
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
                if (user != null)
                    ViewData["userData"] = user.Username;
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

        // GET: FoodController
        [HttpGet]
        public ActionResult Index()
        {
            user_init();
            return View();
        }
        public ActionResult Cur()
        {
            user_init();
            List<Curier> curiers = _foodDeliveryContext.Curiers.ToList();
            return View(curiers);
        }
        public ActionResult Cus()
        {
            user_init(); 
            List<Customer> customers = _foodDeliveryContext.Customers.ToList();
            return View(customers);
        }
        public ActionResult Dish()
        {
            user_init(); 
            List<Dish> dishes = _foodDeliveryContext.Dishes.ToList();
            return View(dishes);
        }
        public ActionResult DilL()
        {
            user_init(); 
            List<Deliverylist> deliverylists = _foodDeliveryContext.Deliverylists.ToList();
            return View(deliverylists);
        }
        public ActionResult DOL()
        {
            user_init(); 
            List<DishOrderList> dishOrderList = _foodDeliveryContext.DishOrderLists.ToList();
            return View(dishOrderList);
        }
        public ActionResult OW()
        {
            user_init(); 
            List<Order> order = _foodDeliveryContext.Orders.ToList();
            return View(order);
        }
        public ActionResult Add1()
        {

            user_init(); 
            return View();
        }
        public ActionResult Add2()
        {
            user_init(); 
            return View();
        }
        public ActionResult Add3()
        {
            user_init(); 
            return View();
        }
        public ActionResult Add4()
        {
            user_init();
            return View();
        }
        public ActionResult Add5()
        {
            user_init();
            return View();
        }
        public ActionResult Add6()
        {
            user_init();
            return View();
        }
        public ActionResult Ed1(int id)
        {
            user_init(); 
            Curier cur = _foodDeliveryContext.Curiers.FirstOrDefault(x => x.IdCurier == id);
            return View(cur);
        }
        public ActionResult Ed2(int id)
        {
            user_init(); 
            Customer cus = _foodDeliveryContext.Customers.FirstOrDefault(x => x.IdCustomer == id);
            return View(cus);
        }
        public ActionResult Ed4(int id)
        {
            user_init();
            Dish dis = _foodDeliveryContext.Dishes.FirstOrDefault(x => x.IdDish == id);
            return View(dis);
        }



        [HttpPost]
        public IActionResult Add1(string LN, string FN, string PA, DateTime Borth, string Tel, string SP, string NP, string KP, string PP, string TY)
        {
            Curier cur = new Curier { CurierLastname = LN, CurierFirstname = FN, CurierPatronymic = PA, 
            Birthday = Borth, CurierPhonenumber = Tel, PassportSeries = SP, PassportNumber = NP, PassportIssuedby = KP, PassportDepartment = PP, DeliveryType = TY };
            _foodDeliveryContext.Add(cur);
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/Cur");
        }
        [HttpPost]
        public IActionResult Add2(string LN, string FN, string PA, string Tel, string GR, string UL, int DO, char ST, int KV)
        {
            Customer cus = new Customer { CustomerLastname = LN, CustomerFirstname = FN, CustomerPatronymic = PA, CustomerPhonenumber = Tel, City = GR, Street = UL, HouseNumber = (short)DO, Building = ST, Apartment = (short)KV };
            _foodDeliveryContext.Add(cus);
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/Cus");
        }
        [HttpPost]
        public IActionResult Add3(int PP, int CH, int IK, DateTime TM, string TO, string ST)
        {
            Deliverylist dell = new Deliverylist { IdDeliverylist = PP, IdOrdersFk = CH, IdCurierFk = IK, TimeDelivered = TM, PaymentType = TO, DeliveryCompletion = ST };
            _foodDeliveryContext.Add(dell);
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/DilL");
        }
        [HttpPost]
        public IActionResult Add4(int PP, string NM, int TS)
        {
            Dish dish = new Dish { IdDish = PP, DishName = NM, DishCost = TS };
            _foodDeliveryContext.Add(dish);
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/Dish");
        }
        [HttpPost]
        public IActionResult Add5(int CH, int NM, int KL)
        {
            DishOrderList dishOrderList = new DishOrderList { IdOrdersFk = CH,  IdDishFk= NM, Quantity = KL };
            _foodDeliveryContext.Add(dishOrderList);
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/DOL");
        }
        [HttpPost]
        public IActionResult Add6(int IP, DateTime DT, int IT)
        {
            Order order = new Order {  IdCustomerFk = IP, TimeOrdered = DT, Totalcost = IT };
            _foodDeliveryContext.Add(order);
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/OW");
        }
        [HttpPost]
        public ActionResult Ed1(int Id, string LN, string FN, string PA, DateTime Borth, string Tel, string SP, string NP, string KP, string PP, string TY)
        {

            _foodDeliveryContext.Curiers.Where(x => x.IdCurier == Id).ExecuteUpdate(s => s.SetProperty(u => u.CurierLastname, u => LN).SetProperty(u => u.CurierFirstname, u => FN)
           .SetProperty(u => u.CurierPatronymic, u => PA).SetProperty(u => u.Birthday, u => Borth).SetProperty(u => u.CurierPhonenumber, u => Tel)
           .SetProperty(u => u.PassportSeries, u => SP).SetProperty(u => u.PassportNumber, u => NP).SetProperty(u => u.PassportIssuedby, u => KP)
           .SetProperty(u => u.PassportDepartment, u => PP).SetProperty(u => u.DeliveryType, u => TY));
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/Cur");
        }
        [HttpPost]
        public ActionResult Ed2(int Id, string LN, string FN, string PA, string TEL, string CY, string ST, int HN, string BY, short AP)
        {
            char a = char.Parse(BY);
            _foodDeliveryContext.Customers.Where(x => x.IdCustomer == Id).ExecuteUpdate(s => s.SetProperty(u => u.CustomerLastname, u => LN).SetProperty(u => u.CustomerFirstname, u => FN)
           .SetProperty(u => u.CustomerPatronymic, u => PA).SetProperty(u => u.CustomerPhonenumber, u => TEL).SetProperty(u => u.City, u => CY)
           .SetProperty(u => u.Street, u => ST).SetProperty(u => u.HouseNumber, u => HN).SetProperty(u => u.Building, u => a)
           .SetProperty(u => u.Apartment, u => AP));
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/Cus");
        }
        [HttpPost]
        public ActionResult Ed4(int Id, string NM, string TS)
        {
            decimal a = decimal.Parse(TS);
            _foodDeliveryContext.Dishes.Where(x => x.IdDish == Id).ExecuteUpdate(s => s.SetProperty(u => u.DishName, u => NM).SetProperty(u => u.DishCost, u => a));
            _foodDeliveryContext.SaveChanges();
            return RedirectPermanent("~/Food/Dish");
        }
        public IActionResult Del1(int Id)
        {
            _foodDeliveryContext.Curiers.Where(x => x.IdCurier == Id).ExecuteDelete();
            _foodDeliveryContext.SaveChanges();
            return Redirect("~/Food/Cur");
        }
        public IActionResult Del2(int Id)
        {
            _foodDeliveryContext.Customers.Where(x => x.IdCustomer == Id).ExecuteDelete();
            _foodDeliveryContext.SaveChanges();
            return Redirect("~/Food/Cus");
        }
        public IActionResult Del4(int Id)
        {
            _foodDeliveryContext.Dishes.Where(x => x.IdDish == Id).ExecuteDelete();
            _foodDeliveryContext.SaveChanges();
            return Redirect("~/Food/Dish");
        }

    }

}
