﻿using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.EntityFrameworkCore;
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
        public ProjectRepo(ApplicationDbContext db,IConfiguration config)
        {
            this._db = db;
            this._config = config;
        }
        public Response GetAllProjects()
        {
            Response response = new Response();
            var projects = _db.hbl_tbl_project.ToList();
            if (projects == null) { response.RespMsg = "Project Data Not Found"; response.RespObj = null; return response; }
            else { response.Resp = true; response.RespMsg = "Project Data Found Successfully"; response.RespObj = projects; return response; }
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
                if (string.IsNullOrEmpty(project.name) == null) { response.RespMsg = "Name is Blank";return response; }
                project.name = project.name.Trim().ToLower();
                project.date = DateTime.UtcNow;
                project.is_active = true;
                project.created_on = DateTime.UtcNow;
                project.updated_on = DateTime.UtcNow;
                _db.hbl_tbl_project.Add(project);
                var i = _db.SaveChanges();
                string link = _config["url"];
                link += EncryptString(project.pid.ToString());
                project.link = link;
                _db.hbl_tbl_project.Attach(project);
                _db.Entry(project).State = EntityState.Modified;
                var j = _db.SaveChanges();
                if (j == 0) { response.RespMsg = "Project Not Saved";response.RespObj = null;return response; }
                 response.Resp = true;response.RespMsg = "Project Saved Successfully";return response; 
            }
            catch (Exception ex)
            {
                response.Resp = false;
                response.RespMsg = ex.Message;
                response.RespObj = ex;
                return response;
            }
        }
        public static string EncryptString(string plainText)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = secretKey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }
        public string DecryptString(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = secretKey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }


    }
}
