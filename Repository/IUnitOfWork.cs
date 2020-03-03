







using System;

namespace Repository
{
    public interface IUnitOfWork:IDisposable{



        void Compelete();
    }
}