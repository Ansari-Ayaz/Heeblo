using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Security.Cryptography;
using System.Text;

namespace Heeblo.Implementation
{
    public class ProjectRepo : IProject
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private static byte[] secretKey = Encoding.UTF8.GetBytes("ThisIsASecretKey!");
        private static byte[] iv = Encoding.UTF8.GetBytes("ThisIsAnIV12345");
        public ProjectRepo(ApplicationDbContext db, IConfiguration config)
        {
            this._db = db;
            this._config = config;
        }
        public List<hbl_tbl_project> GetAllProjects(int uid)
        {
            //Response response = new Response();
            //var projects = _db.hbl_tbl_project.Where(z => z.created_by == uid).ToList();
            //if (projects == null) { return null; }
            //return projects;
            List<hbl_tbl_project> allAppsCount = new List<hbl_tbl_project>();
            string connectionString = _config.GetConnectionString("HBL");
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"select a.*,
                               (select count(*) from public.hbl_tbl_application where pid=a.pid)applicatant_count
                               from public.hbl_tbl_project a where a.created_by = @uid";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@uid", uid);
                connection.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        hbl_tbl_project appsCount = new hbl_tbl_project();
                        appsCount.pid = int.Parse(sdr["pid"].ToString());
                        appsCount.name = sdr["name"].ToString();
                        appsCount.link = sdr["link"].ToString();
                        appsCount.date = DateTime.Parse(sdr["date"].ToString());
                        appsCount.is_active = bool.Parse(sdr["is_active"].ToString());
                        appsCount.created_on = DateTime.Parse(sdr["created_on"].ToString());
                        appsCount.created_by = int.Parse(sdr["created_by"].ToString());
                        appsCount.updated_on = DateTime.Parse(sdr["updated_on"].ToString());
                        //appsCount.updated_by = int.Parse(sdr["updated_by"].ToString());
                        appsCount.applicatant_count = int.Parse(sdr["applicatant_count"].ToString());
                        allAppsCount.Add(appsCount);
                    }

                }
            }
            return allAppsCount;
        }

        public Response GetProjectById(int id)
        {
            Response response = new Response();
            var projects = _db.hbl_tbl_project.FirstOrDefault(z => z.pid == id);
            if (projects == null) { response.RespMsg = "Project Data Not Found"; response.RespObj = null; return response; }
            else { response.Resp = true; response.RespMsg = "Project Data Found Successfully"; response.RespObj = projects; return response; }
        }

        public Response SaveProject(hbl_tbl_project project)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrEmpty(project.name) == null) { response.RespMsg = "Name is Blank"; return response; }
                project.name = project.name.Trim().ToLower();
                project.date = DateTime.UtcNow;
                project.is_active = true;
                project.created_on = DateTime.UtcNow;
                project.updated_on = DateTime.UtcNow;
                _db.hbl_tbl_project.Add(project);
                var i = _db.SaveChanges();
                string link = _config["url"] + AESEncryption.Encrypt(project.pid.ToString());
                project.link = link;
                _db.hbl_tbl_project.Attach(project);
                _db.Entry(project).State = EntityState.Modified;
                var j = _db.SaveChanges();
                if (j == 0) { response.RespMsg = "Project Not Saved"; response.RespObj = null; return response; }
                response.Resp = true; response.RespMsg = "Project Saved Successfully"; return response;
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
