

using System.Collections.Generic;
using Models;
using Service;

namespace Services{

    // ByConvention
    public class UserInfoService : ServiceBase<Models.UserInfo,Models.UserInfo, long>
    {
        public UserInfoService()
        {
            
        }
    }
}