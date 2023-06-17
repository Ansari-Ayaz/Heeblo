using Heeblo.Models;
using Heeblo.Repository;

namespace Heeblo.Implementation
{
    public class ApplicationRepo : IApplication
    {
        private readonly ApplicationDbContext _db;

        public ApplicationRepo(ApplicationDbContext db)
        {
            this._db = db;
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
        public Response SaveApplication(hbl_tbl_application application)
        {
            Response response = new Response();
            try
            {
                if (application.pid <= 0) { response.RespMsg = "Project id Required";return response; }
                if (application.uid <= 0) { response.RespMsg = "User id Required"; return response; }
                application.is_active = true;
                application.created_on = DateTime.Now;
                application.updated_on = DateTime.Now;
                application.date = DateTime.Now;
                _db.hbl_tbl_application.Add(application);
                var i = _db.SaveChanges();
                if(i == 0) { response.RespMsg = "Application Failed to Saved";return response; }
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
