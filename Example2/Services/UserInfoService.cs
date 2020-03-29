

using System.Collections.Generic;
using Models;
using EnTier.Service;

namespace Services{

    // ByConvention
    public class UserInfoService : ServiceBase<Models.UserInfo,Models.UserInfo, long>
    {
        public UserInfoService()
        {
            
        }
    }
}