using Heeblo.Implementation;
using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IUser
    {
        List<hbl_tbl_user> GetAllUser(int role);
        Response GetUserByPublication(int pid);
        Response GetUserById(int id);
        Response SaveUser(hbl_tbl_user user);
        Response ForgotLink(string userCred);
        Response PasswordForgot(int uid, string password);
        Response ValidateUser(LoginReq req);
        bool VerifyUser(int uid);
        bool ChangeIsActive(int uid, bool isActive);
    }
}
