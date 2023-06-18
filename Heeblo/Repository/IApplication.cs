using Heeblo.Models;

namespace Heeblo.Repository
{
    public interface IApplication
    {
        Response GetAllApplication();
        Response GetApplicationById(int id);
        Response SaveApplication(IFormFile file1, IFormFile file2, application_view application);
    }
}
