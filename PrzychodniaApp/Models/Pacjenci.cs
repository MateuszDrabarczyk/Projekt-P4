using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class Pacjenci
{
    public int IdPacjenta { get; set; }

    public string Pesel { get; set; } = null!;

    public string Imie { get; set; } = null!;

    public string Nazwisko { get; set; } = null!;

    public string? Telefon { get; set; }

    public string? Adres { get; set; }

    public virtual ICollection<Wizyty> Wizyties { get; set; } = new List<Wizyty>();
}
