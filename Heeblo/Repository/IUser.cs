using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IUser
    {
        Response GetAllUser();
        Response GetUserByPublication(int pid);
        Response GetUserById(int id);
        Response SaveUser(hbl_tbl_user user);
    }
}
