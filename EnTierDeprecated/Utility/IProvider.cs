



using System;

namespace EnTier.Utility{

    public interface IProvider<T> {

        T Create();
    }

    public class Provider<T> : IProvider<T>
    {
        private readonly Func<T> _builder;

        public Provider(Func<T> builder)
        {
            _builder = builder;
        }
        public T Create()
        {
            return _builder();
        }
    }

}