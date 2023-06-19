using Heeblo.Implementation;
using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IUser
    {
        Response GetAllUser();
        Response GetUserByPublication(int pid);
        Response GetUserById(int id);
        Response SaveUser(hbl_tbl_user user);
        Response ForgotLink(string userCred);
        string PasswordForgot(int uid, string password);
        Response ValidateUser(LoginReq req);
        bool VerifyUser(int uid);
    }
}
