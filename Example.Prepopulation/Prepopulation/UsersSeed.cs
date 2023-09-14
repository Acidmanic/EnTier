using System.Collections.Generic;
using EnTier.Prepopulation;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Prepopulation
{
    public class UsersSeed : SeedBase<User>
    {
        public static readonly User Administrator = new User
        {
            Email = "administrator@entier.net",
            Id = 0,
            Username = "administrator",
            FullName = "Acidmanic Moayedi"
        };


        public override IEnumerable<User> SeedingObjects => new[] { Administrator };
    }
}