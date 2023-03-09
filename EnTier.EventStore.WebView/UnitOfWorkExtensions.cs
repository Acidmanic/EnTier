using System;
using System.Linq;
using EnTier.UnitOfWork;

namespace EnTier.EventStore.WebView
{
    public static class UnitOfWorkExtensions
    {



        public static object GetStreamRepository(this IUnitOfWork unitOfWork, Type eventType, Type eventId, Type streamId)
        {
            var uowType = unitOfWork.GetType();

            var method = uowType.GetMethods()
                .FirstOrDefault(m => m.Name == "GetStreamRepository" && m.IsGenericMethod);

            if (method != null)
            {
                var genericMethod = method.MakeGenericMethod(new[] { eventType, eventId, streamId });

                var repository = genericMethod.Invoke(unitOfWork, new object[] { });

                return repository;
            }


            return null;
        }
    }
}