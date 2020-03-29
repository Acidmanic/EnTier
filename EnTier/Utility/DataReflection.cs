


using System;
using System.Reflection;

namespace EnTier.Reflection{

    public class DataReflection{

            public Func<Entity,bool> IdReader<Entity,Tid>(Tid id)
            {
                var idProperty = GetIdProperty<Entity,Tid>();

                if (idProperty != null)
                {
                    return (Entity e) => {
                        try
                        {
                            var eId = (Tid)idProperty.GetValue(e);

                            return eId.Equals(id);
                        }
                        catch (Exception){}

                        return false;
                    };
                }


                return e => false;
            }

            public PropertyInfo GetIdProperty<Entity,Tid>(){
                
                var idType = typeof(Tid);

                

                var entityType = typeof(Entity);

                var idProperty = entityType.GetProperty("Id", idType);

                return idProperty;
            }


            public void UseId<Entity,Tid>(Entity entity,Action<Tid> usage) {
                
                var idProperty = GetIdProperty<Entity,Tid>();

                try
                {
                    var id = (Tid)idProperty.GetValue(entity);

                    usage(id);
                }
                catch (Exception){ }
            }

            public void SetId<SrcEntity,Tid,DstEntity>(SrcEntity src,DstEntity dst){
                
                var srcProperty = GetIdProperty<SrcEntity,Tid>();

                var dstProperty = GetIdProperty<DstEntity,Tid>();

                try
                {
                    var id = (Tid)srcProperty.GetValue(src);

                    dstProperty.SetValue(dst,id);
                }
                catch (Exception){            }

            }
    }

}