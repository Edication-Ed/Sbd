using System;
using System.Collections.Generic;

namespace Food_Delivery.Models;

public partial class DishOrderList
{
    public int IdDishOrderList { get; set; }

    public int IdOrdersFk { get; set; }

    public int IdDishFk { get; set; }

    public int Quantity { get; set; }

    public virtual Dish IdDishFkNavigation { get; set; } = null!;

    public virtual Order IdOrdersFkNavigation { get; set; } = null!;
}
