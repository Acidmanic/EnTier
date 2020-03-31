



using Models;
using EnTier.Binding;
using EnTier.Binding.Abstractions;

namespace Mapping{



    public class ObjectMapper : IObjectMapper
    {
        public TDestination Map<TDestination>(object src)
        {
            return (TDestination) src;
        }

        public void Map<TDestination, TSource>(TSource src, TDestination dst)
        {
            var s = (UserInfo) (object) src;
            var d = (UserInfo) (object) dst;

            d.Biography = s.Biography;
            d.BirthDate = s.BirthDate;
            d.Id=s.Id;
            d.IdCardSerial=s.IdCardSerial;
            d.Name=s.Name;
            d.RegistrationDate=s.RegistrationDate;
            d.Surname=s.Surname;

           
        }
    }
}