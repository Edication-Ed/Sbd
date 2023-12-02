using Food_Delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Delivery
{
    public class Program
    {
        public static WebApplicationBuilder builder = null;
        public static void Main(string[] args)
        {
            builder = WebApplication.CreateBuilder(args);
            string con = builder.Configuration.GetConnectionString("DefoultConnection");
            builder.Services.AddDbContext<FoodDeliveryContext>(option => option.UseNpgsql(con));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}");

            app.Run();
        }
    }
}