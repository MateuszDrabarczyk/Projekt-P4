using System;
using System.Collections.Generic;

namespace PrzychodniaApp.Models;

public partial class WizytyUslugi
{
    public int IdPozycji { get; set; }

    public int IdWizyty { get; set; }

    public int IdUslugi { get; set; }

    public decimal CenaHistoryczna { get; set; }

    public virtual SlUslugi IdUslugiNavigation { get; set; } = null!;

    public virtual Wizyty IdWizytyNavigation { get; set; } = null!;
}
