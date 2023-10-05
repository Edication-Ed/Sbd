using System;
using System.Collections.Generic;
using Food_Delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Delivery;

public partial class FoodDeliveryContext : DbContext
{
    public FoodDeliveryContext()
    {
    }

    public FoodDeliveryContext(DbContextOptions<FoodDeliveryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Curier> Curiers { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Deliverylist> Deliverylists { get; set; }

    public virtual DbSet<Dish> Dishes { get; set; }

    public virtual DbSet<DishOrderList> DishOrderLists { get; set; }

    public virtual DbSet<DishOrderListView> DishOrderListViews { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersView> OrdersViews { get; set; }

    public virtual DbSet<Userlogin> Userlogins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Food_Delivery;Username=postgres;Password=Dima2023");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Curier>(entity =>
        {
            entity.HasKey(e => e.IdCurier).HasName("curier_pkey");

            entity.ToTable("curier");

            entity.Property(e => e.IdCurier)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_curier");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.CurierFirstname)
                .HasMaxLength(80)
                .HasDefaultValueSql("'Петр'::character varying")
                .HasColumnName("curier_firstname");
            entity.Property(e => e.CurierLastname)
                .HasMaxLength(80)
                .HasDefaultValueSql("'Петров'::character varying")
                .HasColumnName("curier_lastname");
            entity.Property(e => e.CurierPatronymic)
                .HasMaxLength(80)
                .HasColumnName("curier_patronymic");
            entity.Property(e => e.CurierPhonenumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("curier_phonenumber");
            entity.Property(e => e.DeliveryType)
                .HasMaxLength(10)
                .HasColumnName("delivery_type");
            entity.Property(e => e.PassportDepartment)
                .HasMaxLength(7)
                .IsFixedLength()
                .HasColumnName("passport_department");
            entity.Property(e => e.PassportIssuedby)
                .HasMaxLength(255)
                .HasColumnName("passport_issuedby");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("passport_number");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("passport_series");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.IdCustomer).HasName("client_pkey");

            entity.ToTable("customer");

            entity.Property(e => e.IdCustomer)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_customer");
            entity.Property(e => e.Apartment).HasColumnName("apartment");
            entity.Property(e => e.Building)
                .HasMaxLength(1)
                .HasColumnName("building");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.CustomerFirstname)
                .HasMaxLength(80)
                .HasDefaultValueSql("'Иван'::character varying")
                .HasColumnName("customer_firstname");
            entity.Property(e => e.CustomerLastname)
                .HasMaxLength(80)
                .HasDefaultValueSql("'Иванов'::character varying")
                .HasColumnName("customer_lastname");
            entity.Property(e => e.CustomerPatronymic)
                .HasMaxLength(80)
                .HasColumnName("customer_patronymic");
            entity.Property(e => e.CustomerPhonenumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("customer_phonenumber");
            entity.Property(e => e.HouseNumber).HasColumnName("house_number");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .HasColumnName("street");
        });

        modelBuilder.Entity<Deliverylist>(entity =>
        {
            entity.HasKey(e => e.IdDeliverylist).HasName("deliverylist_pkey");

            entity.ToTable("deliverylist");

            entity.Property(e => e.IdDeliverylist)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_deliverylist");
            entity.Property(e => e.DeliveryCompletion)
                .HasMaxLength(6)
                .HasColumnName("delivery_completion");
            entity.Property(e => e.IdCurierFk).HasColumnName("id_curier_fk");
            entity.Property(e => e.IdOrdersFk).HasColumnName("id_orders_fk");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(11)
                .HasColumnName("payment_type");
            entity.Property(e => e.TimeDelivered)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_delivered");

            entity.HasOne(d => d.IdCurierFkNavigation).WithMany(p => p.Deliverylists)
                .HasForeignKey(d => d.IdCurierFk)
                .HasConstraintName("deliverylist_id_curier_fk_fkey");

            entity.HasOne(d => d.IdOrdersFkNavigation).WithMany(p => p.Deliverylists)
                .HasForeignKey(d => d.IdOrdersFk)
                .HasConstraintName("deliverylist_id_orders_fk_fkey");
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.IdDish).HasName("dish_pkey");

            entity.ToTable("dish");

            entity.HasIndex(e => e.DishName, "dish_dish_name_key").IsUnique();

            entity.Property(e => e.IdDish)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_dish");
            entity.Property(e => e.DishCost)
                .HasPrecision(10, 2)
                .HasColumnName("dish_cost");
            entity.Property(e => e.DishName)
                .HasMaxLength(80)
                .HasColumnName("dish_name");
        });

        modelBuilder.Entity<DishOrderList>(entity =>
        {
            entity.HasKey(e => e.IdDishOrderList).HasName("dish_order_list_pkey");

            entity.ToTable("dish_order_list");

            entity.Property(e => e.IdDishOrderList)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_dish_order_list");
            entity.Property(e => e.IdDishFk).HasColumnName("id_dish_fk");
            entity.Property(e => e.IdOrdersFk).HasColumnName("id_orders_fk");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IdDishFkNavigation).WithMany(p => p.DishOrderLists)
                .HasForeignKey(d => d.IdDishFk)
                .HasConstraintName("dish_order_list_id_dish_fk_fkey");

            entity.HasOne(d => d.IdOrdersFkNavigation).WithMany(p => p.DishOrderLists)
                .HasForeignKey(d => d.IdOrdersFk)
                .HasConstraintName("dish_order_list_id_orders_fk_fkey");
        });

        modelBuilder.Entity<DishOrderListView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("dish_order_list_view");

            entity.Property(e => e.DishName)
                .HasMaxLength(80)
                .HasColumnName("dish_name");
            entity.Property(e => e.IdDishOrderList).HasColumnName("id_dish_order_list");
            entity.Property(e => e.IdOrdersFk).HasColumnName("id_orders_fk");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrders).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.IdOrders)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_orders");
            entity.Property(e => e.IdCustomerFk).HasColumnName("id_customer_fk");
            entity.Property(e => e.TimeOrdered)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_ordered");
            entity.Property(e => e.Totalcost)
                .HasPrecision(10, 2)
                .HasColumnName("totalcost");

            entity.HasOne(d => d.IdCustomerFkNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdCustomerFk)
                .HasConstraintName("fk_id_customer");
        });

        modelBuilder.Entity<OrdersView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("orders_view");

            entity.Property(e => e.CustomerFirstname)
                .HasMaxLength(80)
                .HasColumnName("customer_firstname");
            entity.Property(e => e.CustomerLastname)
                .HasMaxLength(80)
                .HasColumnName("customer_lastname");
            entity.Property(e => e.CustomerPatronymic)
                .HasMaxLength(80)
                .HasColumnName("customer_patronymic");
            entity.Property(e => e.IdOrders).HasColumnName("id_orders");
            entity.Property(e => e.TimeOrdered)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_ordered");
            entity.Property(e => e.Totalcost)
                .HasPrecision(10, 2)
                .HasColumnName("totalcost");
        });

        modelBuilder.Entity<Userlogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userlogin_pkey");

            entity.ToTable("userlogin");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Additionalid)
                .HasDefaultValueSql("0")
                .HasColumnName("additionalid");
            entity.Property(e => e.Passcode)
                .HasMaxLength(64)
                .HasColumnName("passcode");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("1")
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(64)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
