using System.Collections.Generic;
using Acidmanic.Utilities.Results;
using EnTier.Prepopulation.Contracts;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Prepopulation
{
    public class UsersSeed : ISeed<User>
    {
        public static readonly User Administrator = new User
        {
            Email = "administrator@entier.net",
            Id = 0,
            Username = "administrator",
            FullName = "Acidmanic Moayedi"
        };


        public string SeedName => "Users";

        public IEnumerable<User> SeedingObjects => new[] { Administrator };

        public Result<ISeedingHook<User>> HooksIntoSeedingBehavior =>
            new Result<ISeedingHook<User>>().FailAndDefaultValue();
    }
}