using Acidmanic.Utilities.Filtering.Attributes;
using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.Meadow.Models;

public class Media
{
    
    [UniqueMember]
    [AutoValuedMember]
    public long Id { get; set; }
    
    [FilterField]
    public string Name { get; set; }
    
    [FilterField]
    public string Url { get; set; }
}