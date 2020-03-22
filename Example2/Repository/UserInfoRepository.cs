



using System.Collections.Generic;
using Models;
using System.Linq;
using DIBinding;

namespace Repository{



    
    public interface IUserInfoRepository{}

    [InjectionEntry(typeof(IUserInfoRepository))]
    public class UserInfoReposiroty :IUserInfoRepository, IRepository<UserInfo>
    {


        private static readonly List<UserInfo> _users = new List<UserInfo>();


        public UserInfo Add(UserInfo value)
        {

            value.Id = _users.Count;

            _users.Add(value);

            return value;
        }

        public List<UserInfo> Find(System.Func<UserInfo, bool> condition)
        {
            return _users.FindAll( model => condition(model));
        }

        public List<UserInfo> GetAll()
        {
            var ret = new List<UserInfo>();

            ret.AddRange(_users);

            return ret;
        }

        public UserInfo GetById(long id)
        {
            return _users.Find(u => u.Id==id);
        }

        public UserInfo GetById(UserInfo entity)
        {
            var index = IndexOf(entity);

            if(index >-1){
                var ret = _users[index];

                return ret;
            }

            return null;
        }


        public UserInfo Remove(UserInfo value)
        {
            var index = IndexOf(value);

            if(index >-1){
                var ret = _users[index];

                _users.RemoveAt(index);

                return ret;
            }

            return null;
        }

        private int IndexOf(UserInfo user){

            return IndexOf(user.Id);
        }

        private int IndexOf(long userId){

            for(var i =0;i<_users.Count;i++){
                var u = _users[i];

                if(u.Id == userId){
                    return i;
                }
            }

            return -1;
        }

        public UserInfo RemoveById(long id)
        {
                
            var index = IndexOf(id);

            if(index >-1){
                var ret = _users[index];

                _users.RemoveAt(index);

                return ret;
            }

            return null;
        }
    }

}