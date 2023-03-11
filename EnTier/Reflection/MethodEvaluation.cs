using System;
using System.Reflection;

namespace EnTier.Reflection
{

    public class MethodEvaluation
    {
        public bool IsSyncVoid { get; set; }

        public bool IsAsyncVoid { get; set; }

        public bool IsAsyncFunction { get; set; }

        public bool IsAsync => IsAsyncFunction || IsAsyncVoid;

        public bool IsSyncFunction => !IsAsync && !IsSyncVoid;

        public Type SyncReturnType { get; set; }

        public Type AsyncReturnType { get; set; }

        public bool IsFunction => IsAsyncFunction || IsSyncFunction;

        public Type OverAllReturnValue => IsAsync ? AsyncReturnType : SyncReturnType;
        public PropertyInfo ResultProperty { get; set; }
    }
}