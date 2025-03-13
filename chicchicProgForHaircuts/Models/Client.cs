using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Client
{
    public int Id { get; set; }

    public string NameClient { get; set; } = null!;

    public string SurnameClient { get; set; } = null!;

    public string PatronymicClient { get; set; } = null!;

    public int? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public int? VisitCount { get; set; }

    public int? StatusId { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Gender? GenderNavigation { get; set; }

    public virtual Clientstatus? Status { get; set; }
}
