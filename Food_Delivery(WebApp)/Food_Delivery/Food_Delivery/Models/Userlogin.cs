using System;
using System.Collections.Generic;

namespace Food_Delivery;

public partial class Userlogin
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Passcode { get; set; } = null!;

    public int? Status { get; set; }

    public int? Additionalid { get; set; }
}
