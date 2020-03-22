



using Repository;
using Utility;

namespace Providers{




    public class NoneContexedGenericUnitOfWorkProvider : IProvider<IUnitOfWork>,IEnTierInternal
    {
        public IUnitOfWork Create()
        {
            return new NoneContexedGenericUnitOfWork();
        }
    }
}