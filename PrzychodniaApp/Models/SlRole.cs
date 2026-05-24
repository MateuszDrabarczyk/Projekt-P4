using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class SlRole
{
    public int IdRoli { get; set; }

    public string Nazwa { get; set; } = null!;

    public virtual ICollection<Uzytkownicy> Uzytkownicies { get; set; } = new List<Uzytkownicy>();
}
