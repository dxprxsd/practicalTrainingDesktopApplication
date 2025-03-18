using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Appointmentsstatus
{
    public int Id { get; set; }

    public string? StatusapName { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
