using System;
using System.Collections.Generic;

namespace Food_Delivery;

public partial class Dish
{
    public int IdDish { get; set; }

    public string DishName { get; set; } = null!;

    public decimal DishCost { get; set; }

    public virtual ICollection<DishOrderList> DishOrderLists { get; set; } = new List<DishOrderList>();
}
