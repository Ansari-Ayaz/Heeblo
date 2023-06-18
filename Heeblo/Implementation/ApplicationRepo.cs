using Heeblo.Models;
using Heeblo.Repository;

namespace Heeblo.Implementation
{
    public class ApplicationRepo : IApplication
    {
        private readonly ApplicationDbContext _db;
        private readonly IHeeblo _heeblo;

        public ApplicationRepo(ApplicationDbContext db,IHeeblo heeblo)
        {
            this._db = db;
            this._heeblo = heeblo;
        }
        public Response GetAllApplication()
        {
            Response response = new Response();
            var applns = _db.hbl_tbl_application.ToList();
            if (applns.Count == 0) { response.RespMsg = "Application Data Not Found";response.RespObj = null;return response;}
            response.Resp = true;response.RespMsg = "Application Data Found Successfully";response.RespObj = applns;return response;
        }
        public Response GetApplicationById(int id)
        {
            Response response = new Response();
            var appln = _db.hbl_tbl_application.FirstOrDefault(z => z.application_id == id);
            if (appln == null) { response.RespMsg = "Application Not Found On This Id";return response; }
            response.Resp = true;response.RespMsg = "Application Found Successfully";response.RespObj= appln;return response;
        }
        public Response SaveApplication(IFormFile file1,IFormFile file2, application_view application)
        {
            Response response = new Response();
            try
            {
                hbl_tbl_application app = new hbl_tbl_application();
                if (application.pid <= 0) { response.RespMsg = "Project id Required";return response; }
                if (application.uid <= 0) { response.RespMsg = "User id Required"; return response; }
                app.pid = application.pid;
                app.uid = application.uid;
                app.updated_by = application.created_by;
                app.created_by = application.created_by;
                app.is_active = true;
                app.created_on = DateTime.Now;
                app.updated_on = DateTime.Now;
                app.date = DateTime.Now;
                _db.hbl_tbl_application.Add(app);
                var i = _db.SaveChanges();
                Task.Run(() => { _heeblo.GetScores(app.sample_content, app.application_id); });
                if (i == 0) { response.RespMsg = "Application Failed to Saved";return response; }
                return response;
            }
            catch (Exception ex)
            {
                response.Resp = false;
                response.RespMsg = ex.Message;
                response.RespObj = ex;
                return response;
            }
        }
    }
}
