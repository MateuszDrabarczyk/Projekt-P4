using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class Wizyty
{
    public int IdWizyty { get; set; }

    public int IdPacjenta { get; set; }

    public int IdLekarza { get; set; }

    public int IdStatusu { get; set; }

    public string? KodIcd10 { get; set; }

    public DateTime DataStart { get; set; }

    public DateTime DataKoniec { get; set; }

    public string? Zalecenia { get; set; }

    public virtual Lekarze IdLekarzaNavigation { get; set; } = null!;

    public virtual Pacjenci IdPacjentaNavigation { get; set; } = null!;

    public virtual SlStatusy IdStatusuNavigation { get; set; } = null!;

    public virtual SlIcd10? KodIcd10Navigation { get; set; }

    public virtual ICollection<WizytyUslugi> WizytyUslugis { get; set; } = new List<WizytyUslugi>();
}
