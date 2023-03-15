using System.Collections.Generic;
using System.Linq;
using EnTier.EventSourcing.Models;

namespace EnTier.TransferModels
{
    public class MethodDeclaration
    {
        public string Name { get; set; }

        public List<ParameterDeclaration> Parameters { get; set; }

        public MethodDeclaration Load(MethodProfile profile)
        {
            Name = profile.Name;
            Parameters = profile.Method.GetParameters()
                .Select(p => new ParameterDeclaration
                {
                    Name = p.Name,
                    Type = p.ParameterType.Name
                }).ToList();
            return this;
        }
    }
}