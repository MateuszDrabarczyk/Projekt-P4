using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class Lekarze
{
    public int IdUzytkownika { get; set; }

    public string NumerPwz { get; set; } = null!;

    public virtual Uzytkownicy IdUzytkownikaNavigation { get; set; } = null!;

    public virtual ICollection<Wizyty> Wizyties { get; set; } = new List<Wizyty>();
}
