using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class SlUslugi
{
    public int IdUslugi { get; set; }

    public string Nazwa { get; set; } = null!;

    public decimal CenaAktualna { get; set; }

    public virtual ICollection<WizytyUslugi> WizytyUslugis { get; set; } = new List<WizytyUslugi>();
}
