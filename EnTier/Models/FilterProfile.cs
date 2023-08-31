using System.Collections.Generic;

namespace EnTier.Models;

public class FilterProfile
{
    public string FieldName { get; set; }
    
    public bool FullTreeAccess { get; set; }
    
    public string Es6Type { get; set; }
    
    public List<string> AvailableValues { get; set; }
    
    public bool FilterByAvailableValues { get; set; }
    
    public string MaximumValue { get; set; }
    
    public string MinimumValue { get; set; }
}