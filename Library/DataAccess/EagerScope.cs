


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace DataAccess{



    public class EagerScope{


        private Dictionary<Type, List<object>> _actions = new Dictionary<Type, List<Object>>();

        public EagerScope Mark<Entity>(Func<IQueryable<Entity>,IQueryable<Entity>> action)
        {
            var type = typeof(Entity);

            List<object> list;

            if (!_actions.ContainsKey(type))
            {
                list = new List<Object>();

                _actions.Add(type, list);
            }
            else
            {
                list = _actions[type];
            }

            list.Add(action);

            return this;
        }


        /// Applies all eager configurations on given IQueryable object
        /// Returns true if any configuration was registered, false if none.
        public OperationResult<IQueryable<Entity>> Apply<Entity>(IQueryable<Entity> subject)
        {
            var type = typeof(Entity);

            var ret = new OperationResult<IQueryable<Entity>>();

            ret.Success = false;

            ret.Value = subject;

            if (_actions.ContainsKey(type))
            {
                var list = _actions[type];

                list.ForEach(item => {
                    var f = (Func<IQueryable<Entity>,IQueryable<Entity>>) item;

                    ret.Value = f(ret.Value);
                });

                ret.Success = list.Count>0;
            }

            return ret;
            
        }


    }
}