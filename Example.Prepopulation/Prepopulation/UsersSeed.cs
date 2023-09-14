using System.Collections.Generic;
using EnTier.Prepopulation;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Prepopulation
{
    public class UsersSeed : SeedBase<User>
    {
        private readonly IUserNameProvider _userNameProvider;

        public static readonly User Administrator = new User
        {
            Email = "administrator@entier.net",
            Id = 0,
            // Username = "administrator",
            // FullName = "Acidmanic Moayedi"
        };

        public UsersSeed(IUserNameProvider userNameProvider)
        {
            _userNameProvider = userNameProvider;
        }


        public override IEnumerable<User> SeedingObjects => new[] { Administrator };

        public override void Initialize()
        {
            Administrator.FullName = _userNameProvider.FullName();
            Administrator.Username = _userNameProvider.Username();
        }
    }
}