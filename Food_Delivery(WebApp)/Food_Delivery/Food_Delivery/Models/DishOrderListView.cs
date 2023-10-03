using System;
using System.Collections.Generic;

namespace Food_Delivery;

public partial class DishOrderListView
{
    public int? IdDishOrderList { get; set; }

    public int? IdOrdersFk { get; set; }

    public string? DishName { get; set; }

    public int? Quantity { get; set; }
}
