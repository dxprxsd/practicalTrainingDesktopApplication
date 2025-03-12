using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string NameEmployee { get; set; } = null!;

    public string SurnameEmployee { get; set; } = null!;

    public string PatronymicEmployee { get; set; } = null!;

    public int? Gender { get; set; }

    public int? RoleId { get; set; }

    public string? ContactInfo { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Gender? GenderNavigation { get; set; }

    public virtual Role? Role { get; set; }
}
