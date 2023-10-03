using System;
using System.Collections.Generic;

namespace Food_Delivery;

public partial class Customer
{
    public int IdCustomer { get; set; }

    public string CustomerLastname { get; set; } = null!;

    public string CustomerFirstname { get; set; } = null!;

    public string? CustomerPatronymic { get; set; }

    public string CustomerPhonenumber { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public short HouseNumber { get; set; }

    public char? Building { get; set; }

    public short? Apartment { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
