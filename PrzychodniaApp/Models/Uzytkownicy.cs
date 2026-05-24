using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class Uzytkownicy
{
    public int IdUzytkownika { get; set; }

    public int IdRoli { get; set; }

    public string Login { get; set; } = null!;

    public string HasloHash { get; set; } = null!;

    public bool? CzyAktywny { get; set; }

    public virtual ICollection<Audyt> Audyts { get; set; } = new List<Audyt>();

    public virtual SlRole IdRoliNavigation { get; set; } = null!;

    public virtual Lekarze? Lekarze { get; set; }
}
