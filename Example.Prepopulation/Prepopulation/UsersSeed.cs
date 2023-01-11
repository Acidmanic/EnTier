using Acidmanic.Utilities.Results;
using EnTier.Prepopulation;
using EnTier.UnitOfWork;
using Example.Prepopulation.Models;

namespace Example.Prepopulation.Prepopulation
{
    public class UsersSeed:IPrepopulationSeed
    {


        private readonly IUnitOfWork _unitOfWork;
        
        
        public static readonly User Administrator = new User
        {
            Email = "administrator@entier.net",
            Id = 0,
            Username = "administrator",
            FullName = "Acidmanic Moayedi"
        };

        public UsersSeed(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Result Seed()
        {
            var repository = _unitOfWork.GetCrudRepository<User, long>();

            var inserted = repository.Add(Administrator);

            if (inserted == null)
            {
                return false;
            }

            Administrator.Id = inserted.Id;

            return true;
        }

        public void Clear()
        {
            
        }
    }
}