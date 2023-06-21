using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IApplication
    {
        Response GetAllApplication();
        List<AllApplication> GetApplicationByPid(int pid);
        hbl_tbl_application GetApplicationById(int pid);
        UserDetailsWithUploadDocs GetUserDetailsByAppId(int appid);
        Response SaveApplication(application_view application);
        bool ApplicationStatus(AppFeedback feedback);
        //Response GetAllAppCountByPid(int pid);
    }
}
