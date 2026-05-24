using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class Audyt
{
    public int IdLogu { get; set; }

    public int IdUzytkownika { get; set; }

    public string Akcja { get; set; } = null!;

    public DateTime? DataLogu { get; set; }

    public virtual Uzytkownicy IdUzytkownikaNavigation { get; set; } = null!;
}
