using System;
using System.Collections.Generic;

namespace Food_Delivery.Models;

public partial class Order
{
    public int IdOrders { get; set; }

    public int IdCustomerFk { get; set; }

    public DateTime TimeOrdered { get; set; }

    public decimal Totalcost { get; set; }

    public virtual ICollection<Deliverylist> Deliverylists { get; set; } = new List<Deliverylist>();

    public virtual ICollection<DishOrderList> DishOrderLists { get; set; } = new List<DishOrderList>();

    public virtual Customer IdCustomerFkNavigation { get; set; } = null!;
}
