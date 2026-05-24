using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class SlStatusy
{
    public int IdStatusu { get; set; }

    public string Nazwa { get; set; } = null!;

    public virtual ICollection<Wizyty> Wizyties { get; set; } = new List<Wizyty>();
}
