select * from curier;
select * from customer;
select * from deliverylist;
select * from dish;
select * from dish_order_list;
select * from orders;

--View ORDERS-------------------------------------------------------------------------------------------------------
select o.id_orders, co.customer_lastname, co.customer_firstname, co.customer_patronymic, o.time_ordered, o.totalcost 
from orders as o join customer as co on co.id_customer = o.id_customer_fk;

--View DISH_ORDER_LIST----------------------------------------------------------------------------------------------
select d.id_dish_order_list, d.id_orders_fk, ds.dish_name, d.quantity 
from dish_order_list as d join dish as ds on ds.id_dish = d.id_dish_fk;

create view Orders_view as (select o.id_orders, co.customer_lastname, co.customer_firstname, co.customer_patronymic, o.time_ordered, o.totalcost 
from orders as o join customer as co on co.id_customer = o.id_customer_fk);

create view Dish_Order_List_view as (select d.id_dish_order_list, d.id_orders_fk, ds.dish_name, d.quantity 
from dish_order_list as d join dish as ds on ds.id_dish = d.id_dish_fk);

select * from Dish_Order_List_view;
select * from Orders_view;

