using System;
using System.Collections.Generic;
using System.Linq;

namespace chicchicProgForHaircuts.Models;

public partial class Appointment
{
    public int Id { get; set; }

    public int? ClientId { get; set; }

    public int? EmployeeId { get; set; }

    public int? HaircutId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public double? FinalPrice { get; set; }

    public int? AppointmentsstatusId { get; set; }

    public virtual Appointmentsstatus? Appointmentsstatus { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Haircut? Haircut { get; set; }


}
