using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IApplication
    {
        Response GetAllApplication();
        List<AllApplication> GetApplicationByPid(int pid);
        hbl_tbl_application GetApplicationById(int pid);
        Response SaveApplication(application_view application);
        bool ApplicationStatus(string status, int appId);
    }
}
