using System;
using System.Collections.Generic;

namespace chicchicProgForHaircuts.Models;

public partial class Clientstatus
{
    public int Id { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
