using System;
using System.Collections.Generic;

namespace Food_Delivery.Models;

public partial class Deliverylist
{
    public int IdDeliverylist { get; set; }

    public int IdOrdersFk { get; set; }

    public int IdCurierFk { get; set; }

    public DateTime? TimeDelivered { get; set; }

    public string PaymentType { get; set; } = null!;

    public string? DeliveryCompletion { get; set; }

    public virtual Curier IdCurierFkNavigation { get; set; } = null!;

    public virtual Order IdOrdersFkNavigation { get; set; } = null!;
}
