



using Repository;
using Utility;

namespace Providers{




    public class NoneContexedGenericUnitOfWorkProvider : IProvider<IUnitOfWork>
    {
        public IUnitOfWork Create()
        {
            return new NoneContexedGenericUnitOfWork();
        }
    }
}