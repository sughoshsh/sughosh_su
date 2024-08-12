using System;
using System.Collections.Generic;

namespace EchoBot.Models;

public partial class ProductDescription
{
    public int ProductDescriptionId { get; set; }

    public string Description { get; set; }

    public Guid Rowguid { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<ProductModelProductDescription> ProductModelProductDescriptions { get; set; } = new List<ProductModelProductDescription>();
}
