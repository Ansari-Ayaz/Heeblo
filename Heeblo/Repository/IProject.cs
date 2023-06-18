using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IProject
    {
        List<hbl_tbl_project> GetAllProjects();
        Response GetProjectById(int id);
        Response SaveProject(hbl_tbl_project project);
    }
}
