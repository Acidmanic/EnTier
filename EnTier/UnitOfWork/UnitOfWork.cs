


using Channels;
using Context;

namespace Repository{


    public class UnitOfWork : IUnitOfWork
    {


        private readonly IContext _context;

        public UnitOfWork(IContext context){
            _context = context;
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

            var factory = new BuilderFactory<StorageEntity,StorageEntity,Tid>();

            var builder = factory.RepositoryBuilder();

            var repository = builder(dataset);

            return repository;

        }
    }
}