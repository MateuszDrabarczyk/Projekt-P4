using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class SlIcd10
{
    public string Kod { get; set; } = null!;

    public string Opis { get; set; } = null!;

    public virtual ICollection<Wizyty> Wizyties { get; set; } = new List<Wizyty>();
}
