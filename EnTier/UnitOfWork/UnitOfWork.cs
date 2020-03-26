


using Components;
using Context;

namespace Repository{


    public class UnitOfWork : IUnitOfWork
    {


        private readonly IContext _context;

        public UnitOfWork(){
            _context = new ComponentProducer().ProduceContext();
        }
        public void Compelete()
        {
            _context.Apply();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IRepository<StorageEntity, Tid> GetRepository<StorageEntity, Tid>() where StorageEntity : class
        {
             var dataset = _context.GetDataset<StorageEntity>();

            var repository = new ComponentProducer().ProduceRepository<StorageEntity, Tid>(dataset);

            return repository;

        }
    }
}