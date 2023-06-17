using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IApplication
    {
        Response GetAllApplication();
        Response GetApplicationById(int id);
        Response SaveApplication(hbl_tbl_application application);
    }
}
