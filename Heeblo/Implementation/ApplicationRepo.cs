using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xceed.Words.NET;

namespace Heeblo.Implementation
{
    public class ApplicationRepo : IApplication
    {
        private readonly ApplicationDbContext _db;
        private readonly IHeeblo _heeblo;
        private readonly IConfiguration _config;

        public ApplicationRepo(ApplicationDbContext db, IHeeblo heeblo,IConfiguration config)
        {
            this._db = db;
            this._heeblo = heeblo;
            this._config = config;
        }
        public Response GetAllApplication()
        {
            Response response = new Response();
            var applns = _db.hbl_tbl_application.ToList();
            if (applns.Count == 0) { response.RespMsg = "Application Data Not Found"; response.RespObj = null; return response; }
            response.Resp = true; response.RespMsg = "Application Data Found Successfully"; response.RespObj = applns; return response;
        }
        public List<AllApplication> GetApplicationByPid(int pid)
        {
            
            try
            {
                List<AllApplication> applns = new List<AllApplication>();
                string connectionString = _config.GetConnectionString("HBL");
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    string sql = @"select application_id , app.created_on ,app.status, u.name as created_by , ai_score,grammar_score, plagiarism 
                                    from hbl_tbl_application app
                                    inner
                                    join hbl_tbl_user u on u.uid = app.created_by
                                    where app.pid = @pid";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@pid", pid);
                    connection.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            applns.Add(new AllApplication
                            {
                                application_id = int.Parse(reader["application_id"].ToString()),
                                ai_score = decimal.Parse(reader["ai_score"].ToString()),
                                created_by = reader["created_by"].ToString(),
                                created_on = DateTime.Parse(reader["created_on"].ToString()),
                                grammar_score = decimal.Parse(reader["grammar_score"].ToString()),
                                plagiarism = decimal.Parse(reader["plagiarism"].ToString()),
                                status = reader["status"].ToString(),

                            });
                        }
                        
                    }

                }
                return applns;
            }
            catch (Exception ex)
            {
                
                return null;
            }

        }
        public hbl_tbl_application GetApplicationById(int id)
        {
            var applns = _db.hbl_tbl_application.FirstOrDefault(z => z.application_id == id);
            if (applns == null) { return null; }
            return applns;
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
        
        public bool ApplicationStatus(string status,int appId)
        {
            bool resp = false;
            string sql = "update hbl_tbl_application set Status='" + status + "' where application_id='" + appId + "'";
            var appData = _db.hbl_tbl_application.FirstOrDefault(z => z.application_id == appId);
            var user = _db.hbl_tbl_user.FirstOrDefault(z => z.uid == appData.uid);
            var subject = "Heeblo assignment Rejected!";
            var body = "Thanks for your submission but your content is not approved. Better luck next time !!";
            using(NpgsqlConnection con = new NpgsqlConnection(_config.GetConnectionString("HBL")))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                bool sentMail = _heeblo.SendEmail(user.email,subject,body);
                resp = (i > 0);
            }
            return resp;
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
