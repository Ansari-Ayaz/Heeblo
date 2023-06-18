using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IProject
    {
        Response GetAllProjects();
        Response GetProjectById(int id);
        Response SaveProject(hbl_tbl_project project);
    }
}
