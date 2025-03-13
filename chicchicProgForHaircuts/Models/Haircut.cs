using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Haircut
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Gender { get; set; }

    public double Price { get; set; }

    public string? Photo { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Haircutsgender? GenderNavigation { get; set; }
}
