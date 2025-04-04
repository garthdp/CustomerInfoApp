using System;
using System.Collections.Generic;

namespace CustomerInfoApp.Models;

public partial class CustomerInfo
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? TelephoneNumber { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactPersonEmail { get; set; }

    public string? Vatnumber { get; set; }
}
