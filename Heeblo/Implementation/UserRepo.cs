using Heeblo.Models;
using Heeblo.Repository;
using System.Security.Cryptography;
using System.Text;

namespace Heeblo.Implementation
{
    public class UserRepo : IUser
    {
        private readonly ApplicationDbContext _db;

        public UserRepo(ApplicationDbContext db)
        {
            this._db = db;
        }
        public Response GetAllUser()
        {
            Response response = new Response();
            var users = _db.hbl_tbl_user.ToList();
            if (users == null) {
                response.RespMsg = "Data Not Found";
                response.RespObj = null;
                return response;
            }
             else{ response.Resp = true;response.RespMsg = "Data Found Successfully";response.RespObj = users; return response; }
        }
        public Response GetUserByPublication(int pid)
        {
            Response response = new Response();
            var user = _db.hbl_tbl_user.FirstOrDefault(z=>z.role.Equals(pid));
            if (user ==null) { response.RespMsg = "User Not Found";return response; }
            else{ response.Resp = true;response.RespMsg = "User Found Successfully";response.RespObj = user;return response; }
        }
        public Response GetUserById(int id)
        {
            Response response = new Response();
            var user = _db.hbl_tbl_user.FirstOrDefault(z => z.uid.Equals(id));
            if (user == null) { response.RespMsg = "User Not Found"; return response; }
            else { response.Resp = true; response.RespMsg = "User Found Successfully"; response.RespObj = user; return response; }
        }
        public Response SaveUser(hbl_tbl_user user)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrEmpty(user.name) == null) { response.RespMsg = "Name is Blank";return response; }
                if (string.IsNullOrEmpty(user.email) == null) { response.RespMsg = "Email is Blank"; return response; }
                if (string.IsNullOrEmpty(user.mobile) == null) { response.RespMsg = "Mobile Number is Blank"; return response; }
                user.name = user.name.Trim().ToLower();
                user.email = user.email.Trim().ToLower();
                string pass = ComputeMD5Hash(user.password);
                user.verified = false;
                user.password = pass;
                user.is_active = true;
                user.update_on = DateTime.UtcNow;
                user.created_on = DateTime.UtcNow;
                _db.hbl_tbl_user.Add(user);
                var i = _db.SaveChanges();
                user.uid = user.uid;
                if(i == 0) { response.RespMsg = "User Not Saved";return response; }
                if (i > 0) { response.Resp = true;response.RespMsg = "User Saved Successfully";response.RespObj = user;return response; }
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


        public Response ValidateUser(LoginReq req)
        {
            Response resp = new Response();
            var isMobile = false;
            if (req.UserCred.Contains("@"))
            {
                isMobile = false;
            }
            else isMobile = true;
            var user = new hbl_tbl_user();
            if(isMobile)
            user = _db.hbl_tbl_user.FirstOrDefault(z => z.mobile.Equals(req.UserCred));
            else
            {
                user = _db.hbl_tbl_user.FirstOrDefault(z => z.email.Equals(req.UserCred));
            }
            if (user == null)
            {
                resp.RespMsg = "Invalid User";
            }
            else
            {
                if(user.password == ComputeMD5Hash(req.UserPwd))
                {
                    user.password = "";
                    resp.Resp = true;
                    resp.RespObj = user;
                    resp.RespMsg = "Valid User";
                }
                else
                {
                    resp.RespObj = null;
                    resp.RespMsg = "Invalid Credentials";
                }
            }
            return resp;
        }

        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" for lowercase hexadecimal
                }

                return builder.ToString();
            }
        }
        //public static string DecryptString(string cipherText, string secretKey)
        //{
        //    //byte[] cipherBytes = new byte[16];

        //    byte[] cipherBytes = Convert.FromBase64String(cipherText);


        //    string plainText = null;

        //    using (Aes aes = Aes.Create())
        //    {
        //        Array.Copy(Encoding.UTF8.GetBytes(secretKey), cipherBytes, Math.Min(cipherBytes.Length, secretKey.Length));
        //        aes.Key = cipherBytes;
        //        aes.IV = new byte[16]; // IV should match the one used for encryption

        //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        //        using (MemoryStream ms = new MemoryStream(cipherBytes))
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader reader = new StreamReader(cs))
        //                {
        //                    plainText = reader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return plainText;
        //}
    }

    public class LoginReq
    {
        public string UserCred { get; set; }
        public string UserPwd { get; set; }
    }
}
