using Acidmanic.Utilities.Reflection.Attributes;

namespace Example.Prepopulation.Models;

public class Chocolate
{
    [AutoValuedMember] [UniqueMember] public long Id { get; set; }
    
    public string ProductName { get; set; }
    
    public string FactoryName { get; set; }
    
    
}