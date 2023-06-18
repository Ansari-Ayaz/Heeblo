using Heeblo.Models;
using Heeblo.Repository;
using Xceed.Words.NET;

namespace Heeblo.Implementation
{
    public class ApplicationRepo : IApplication
    {
        private readonly ApplicationDbContext _db;
        private readonly IHeeblo _heeblo;

        public ApplicationRepo(ApplicationDbContext db, IHeeblo heeblo)
        {
            this._db = db;
            this._heeblo = heeblo;
        }
        public Response GetAllApplication()
        {
            Response response = new Response();
            var applns = _db.hbl_tbl_application.ToList();
            if (applns.Count == 0) { response.RespMsg = "Application Data Not Found"; response.RespObj = null; return response; }
            response.Resp = true; response.RespMsg = "Application Data Found Successfully"; response.RespObj = applns; return response;
        }
        public Response GetApplicationById(int id)
        {
            Response response = new Response();
            var appln = _db.hbl_tbl_application.FirstOrDefault(z => z.application_id == id);
            if (appln == null) { response.RespMsg = "Application Not Found On This Id"; return response; }
            response.Resp = true; response.RespMsg = "Application Found Successfully"; response.RespObj = appln; return response;
        }
        public Response SaveApplication(application_view application)
        {
            Response response = new Response();
            try
            {
                hbl_tbl_application app = new hbl_tbl_application();
                if (application.pid <= 0) { response.RespMsg = "Project id Required"; return response; }
                if (application.uid <= 0) { response.RespMsg = "User id Required"; return response; }
                if (!PdfIsValid(application.resume)) { response.RespMsg = "Resume should be PDF"; return response; }
                if (!DocIsValid(application.sample_content)) { response.RespMsg = "Sample Content should be Doc or Docx"; return response; }
                app.pid = application.pid;
                app.uid = application.uid;
                app.updated_by = application.created_by;
                app.created_by = application.created_by;
                app.is_active = true;
                app.created_on = DateTime.UtcNow;
                app.updated_on = DateTime.UtcNow;
                app.date = DateTime.UtcNow;

                _db.hbl_tbl_application.Add(app);
                var i = _db.SaveChanges();
                string content = ReadDocFileContent(application.sample_content);
                System.Threading.Tasks.Task.Run(() => { _heeblo.GetScores(content, app.application_id); });
                //_heeblo.GetScores(ReadDocFileContent(application.sample_content), app.application_id);
                if (i == 0)
                {
                    response.RespMsg = "Application Failed to Saved"; return response;

                }
                hbl_tbl_attachment attachment = new hbl_tbl_attachment();
                attachment.application_id = app.application_id;
                attachment.resume = ConvertFiletoBase64(application.resume);
                attachment.sample_content = ConvertFiletoBase64(application.sample_content);
                _db.hbl_tbl_attachment.Add(attachment);
                if (_db.SaveChanges() == 0)
                {
                    response.RespMsg = "Attachment Failed to Saved"; return response;
                }

                response.Resp = true; response.RespMsg = "Application and Attachment Saved Successfully"; response.RespObj = application; return response;
            }
            catch (Exception ex)
            {
                response.Resp = false;
                response.RespMsg = ex.Message;
                response.RespObj = ex;
                return response;
            }
        }

        private string ConvertFiletoBase64(IFormFile formFile)
        {
            string s = null;

            if (formFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    s = Convert.ToBase64String(fileBytes);
                }
            }
            return s;
        }

        private bool PdfIsValid(IFormFile formFile)
        {            // Check if the file is not null and has a PDF file extension
            if (formFile != null && Path.GetExtension(formFile.FileName).ToLower() == ".pdf")
            {
                return true;
            }
            return false;

        }

        private bool DocIsValid(IFormFile formFile)
        {
            if (formFile != null && (Path.GetExtension(formFile.FileName).ToLower() == ".doc" || Path.GetExtension(formFile.FileName).ToLower() == ".docx"))
            {
                return true;
            }
            return false;

        }

        private string ReadDocFileContent(IFormFile file)
        {

            using (var stream = file.OpenReadStream())
            {
                var doc = DocX.Load(stream);
                string content = doc.Text;

                return content;
            }

        }
    }
}
