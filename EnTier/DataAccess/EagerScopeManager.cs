



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Repository;

namespace DataAccess{

    public class EagerScopeManager:IDisposable{

        private class BrowsableStack<T>{

            private readonly List<T> _objects;

            public BrowsableStack()
            {
                _objects = new List<T>();
            }

            public void Push(T obj){
                _objects.Add(obj);
            }

            public T Pop(){
                if(_objects.Count >0){
                    var index = _objects.Count-1;
                    
                    var ret = _objects[index];

                    _objects.RemoveAt(index);

                    return ret;

                }

                return default;
            }

            public int Count(){
                return _objects.Count;
            }

            public T Get(int index){
                return _objects[index];
            }

        }


        private static readonly Stack<EagerScope> _scopes = new Stack<EagerScope>();

        private static EagerScope _currentScope =null;
        public EagerScopeManager(){
            _currentScope = new EagerScope();
            
            _scopes.Push(_currentScope);

        }

        public EagerScopeManager Mark<Entity>(Func<IQueryable<Entity>,IQueryable<Entity>> action){

            _currentScope.Mark(action);            

            return this;
        }

        public EagerScopeManager Mark<Entity>(){

            _currentScope.Mark<Entity>();            

            return this;
        }
        
        public void Dispose()
        {
            _currentScope = _scopes.Pop();
        }

        internal static IQueryable<Entity> Apply<Entity>(IQueryable<Entity> subject){

            for(var i =0;i<_scopes.Count;i++){

                var result = _scopes.ElementAt(i).Apply(subject);
                
                if(result.Success){
                    return result.Value;
                }
            }

            return subject;
        }
    }
}