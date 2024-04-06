using KPILib.Models;
using System.Collections.Generic;
using System.Linq;

namespace KPILib
{
    public static class AuthorizationProcess
    {
        public static bool IsUserAuthorize(string token, string methodType, string menuCode)
        {
            var userRights = CacheManager.GetCache(token);
            if (userRights != null)
            {
                var tokenData = (List<UserRoleRights>)userRights;
                return IsMenuAccessible(methodType, menuCode, tokenData);
            }
            return false; 
        }

        private static bool IsMenuAccessible(string methodType, string menuCode, List<UserRoleRights> roleRights)
        {
            UserRoleRights data;
            switch (methodType.ToUpper())
            {
                case "GET":

                    data = roleRights.FirstOrDefault(x => x.MenuCode == menuCode);
                    if (data != null && data.View)
                    {
                        return true;
                    }

                    break;
                case "POST":

                    data = roleRights.FirstOrDefault(x => x.MenuCode == menuCode);
                    if (data != null && data.Add)
                    {
                        return true;
                    }

                    break;
                case "PUT":

                    data = roleRights.FirstOrDefault(x => x.MenuCode == menuCode);
                    if (data != null && data.Update)
                    {
                        return true;
                    }

                    break;
                case "DELETE":

                    data = roleRights.FirstOrDefault(x => x.MenuCode == menuCode);
                    if (data != null && data.Delete)
                    {
                        return true;
                    }

                    break;
                default:
                    break;
            }

            return false;
        }
    }
}
