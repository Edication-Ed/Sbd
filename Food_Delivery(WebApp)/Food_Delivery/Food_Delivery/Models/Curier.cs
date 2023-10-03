using System;
using System.Collections.Generic;

namespace Food_Delivery;

public partial class Curier
{
    public int IdCurier { get; set; }

    public string CurierLastname { get; set; } = null!;

    public string CurierFirstname { get; set; } = null!;

    public string? CurierPatronymic { get; set; }

    public string CurierPhonenumber { get; set; } = null!;

    public string DeliveryType { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string PassportSeries { get; set; } = null!;

    public string PassportNumber { get; set; } = null!;

    public string PassportIssuedby { get; set; } = null!;

    public string PassportDepartment { get; set; } = null!;

    public virtual ICollection<Deliverylist> Deliverylists { get; set; } = new List<Deliverylist>();
}
