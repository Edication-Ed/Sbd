using System;
using System.Collections.Generic;

namespace Food_Delivery;

public partial class OrdersView
{
    public int? IdOrders { get; set; }

    public int? IdCustomer { get; set; }

    public string? CustomerLastname { get; set; }

    public string? CustomerFirstname { get; set; }

    public string? CustomerPatronymic { get; set; }

    public DateTime? TimeOrdered { get; set; }

    public decimal? Totalcost { get; set; }
}
