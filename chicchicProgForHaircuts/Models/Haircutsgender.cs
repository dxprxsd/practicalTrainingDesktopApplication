using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Haircutsgender
{
    public int Id { get; set; }

    public string HairgenderName { get; set; } = null!;

    public virtual ICollection<Haircut> Haircuts { get; set; } = new List<Haircut>();
}
