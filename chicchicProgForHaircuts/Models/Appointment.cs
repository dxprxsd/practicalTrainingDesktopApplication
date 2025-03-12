using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Appointment
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public int? EmployeeId { get; set; }

    public int? HaircutId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public decimal FinalPrice { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Haircut? Haircut { get; set; }
}
