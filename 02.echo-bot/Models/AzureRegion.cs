using System;
using System.Collections.Generic;

namespace EchoBot.Models;

public partial class AzureRegion
{
    public int Id { get; set; }

    public string DisplayName { get; set; }

    public string Name { get; set; }

    public string RegionalDisplayName { get; set; }
}
