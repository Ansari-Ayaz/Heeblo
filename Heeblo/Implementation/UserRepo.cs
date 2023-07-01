using Heeblo.Models;
using Heeblo.Repository;
using Npgsql;
using System.Security.Cryptography;
using System.Text;

namespace Heeblo.Implementation
{
    public class UserRepo : IUser
    {
        private readonly ApplicationDbContext _db;
        private readonly IHeeblo _heeblo;
        private readonly IConfiguration _config;

        public UserRepo(ApplicationDbContext db, IHeeblo heeblo, IConfiguration config)
        {
            this._db = db;
            this._heeblo = heeblo;
            this._config = config;
        }
        public List<hbl_tbl_user> GetAllUser(int role)
        {
            var users = (role != 0) ? _db.hbl_tbl_user.Where(z => z.role == role).ToList() : _db.hbl_tbl_user.ToList();
            return users;
        }
        public Response GetUserByPublication(int pid)
        {
            Response response = new Response();
            var user = _db.hbl_tbl_user.FirstOrDefault(z => z.role.Equals(pid));
            if (user == null) { response.RespMsg = "User not found"; return response; }
            else { response.Resp = true; response.RespMsg = "User found successfully"; response.RespObj = user; return response; }
        }
        public Response GetUserById(int id)
        {
            Response response = new Response();
            var user = _db.hbl_tbl_user.FirstOrDefault(z => z.uid.Equals(id));
            if (user == null) { response.RespMsg = "User not found"; return response; }
            else { response.Resp = true; response.RespMsg = "User found successfully"; response.RespObj = user; return response; }
        }
        public Response SaveUser(hbl_tbl_user user)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrEmpty(user.name) == null) { response.RespMsg = "Name is blank"; return response; }
                if (string.IsNullOrEmpty(user.email) == null) { response.RespMsg = "Email is blank"; return response; }
                if (string.IsNullOrEmpty(user.mobile) == null) { response.RespMsg = "Mobile number is blank"; return response; }
                if (_db.hbl_tbl_user.Any(z => z.mobile.Equals(user.mobile))) { response.Resp = false; response.RespMsg = "Mobile number already exist"; response.RespObj = user.mobile; return response; }
                if (_db.hbl_tbl_user.Any(z => z.email.Equals(user.email))) { response.Resp = false; response.RespMsg = "email already exist"; response.RespObj = user.mobile; return response; }
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
                string aesString = AESEncryption.Encrypt(user.uid.ToString());
                var link = _config["verifyUrl"] + aesString;
                var subject = "Heeblo account verification";
                var body = @"Dear " + user.name + ", \r\nPlease click below link to verify your account.\r\n" + link;
                System.Threading.Tasks.Task.Run(() => { _heeblo.SendEmail(user.email, subject, body); });
                if (i == 0) { response.RespMsg = "User not saved"; return response; }
                if (i > 0) { response.Resp = true; response.RespMsg = "User saved and mail sent successfully"; response.RespObj = user; return response; }
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
        public bool VerifyUser(int uid)
        {
            //Response response = new Response();
            var user = _db.hbl_tbl_user.FirstOrDefault(z => z.uid.Equals(uid));
            string sql = "update hbl_tbl_user set verified= true where uid='" + uid + "'";
            using (NpgsqlConnection con = new NpgsqlConnection(_config.GetConnectionString("HBL")))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                if (i == 0) { return false; }
                if (i >= 1) { return true; }
                return false;
            }

        }
        public bool ChangeIsActive(int uid, bool isActive)
        {
            //Response response = new Response();
            var user = _db.hbl_tbl_user.FirstOrDefault(z => z.uid.Equals(uid));
            if (user == null) { return false; }
            string sql = "update hbl_tbl_user set is_active= " + isActive + " where uid='" + uid + "'";
            using (NpgsqlConnection con = new NpgsqlConnection(_config.GetConnectionString("HBL")))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                if (i == 0) { return false; }
                if (i >= 1) { return true; }
                return false;
            }

        }
        public Response ForgotLink(string userCred)
        {
            //if (string.IsNullOrEmpty(email)) { return null; }
            Response resp = new Response();
            var isMobile = false;
            if (userCred.Contains("@"))
            {
                isMobile = false;
            }
            else isMobile = true;
            var user = new hbl_tbl_user();
            if (isMobile)
                user = _db.hbl_tbl_user.FirstOrDefault(z => z.mobile.Equals(userCred));
            else
            {
                user = _db.hbl_tbl_user.FirstOrDefault(z => z.email.Equals(userCred));
            }
            if (user == null)
            {
                resp.Resp = false;
                resp.RespMsg = "Invalid user";
                return resp;
            }
            var subject = "Forgot Password Link";
            var link = _config["forgotUrl"] + AESEncryption.Encrypt(user.uid.ToString());
            var body = @"Dear " + user.name + ", \r\nPlease click below link to reset your password.\r\n" + link;
            System.Threading.Tasks.Task.Run(() => { _heeblo.SendEmail(user.email, subject, body); });
            resp.Resp = true;
            resp.RespMsg = "We have sent a mail to " + user.email;
            return resp;
        }
        public Response PasswordForgot(int uid, string password)
        {
            Response resp = new Response();
            if (uid <= 0)
                if (string.IsNullOrEmpty(password)) { resp.Resp = false; resp.RespMsg = "Password should not blank"; return resp; ; }
            var pass = ComputeMD5Hash(password);
            string sql = "update hbl_tbl_user set password= '" + pass + "' where uid='" + uid + "'";
            using (NpgsqlConnection con = new NpgsqlConnection(_config.GetConnectionString("HBL")))
            {
                NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                if (i == 0) { resp.Resp = false; resp.RespMsg = "Password not updated"; return resp; }
                { resp.Resp = true; resp.RespMsg = "Password updated successfully"; return resp; }
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
            if (isMobile)
                user = _db.hbl_tbl_user.FirstOrDefault(z => z.mobile.Equals(req.UserCred));
            else
            {
                user = _db.hbl_tbl_user.FirstOrDefault(z => z.email.Equals(req.UserCred));
            }
            if (user == null)
            {
                resp.RespMsg = "Invalid user";
            }
            else
            {

                if (user.password == ComputeMD5Hash(req.UserPwd))
                {
                    if (!user.verified)
                    {
                        string aesString = AESEncryption.Encrypt(user.uid.ToString());
                        var link = _config["verifyUrl"] + aesString;
                        var subject = "Heeblo account verification";
                        var body = @"Dear " + user.name + ", \r\nPlease click below link to verify your account.\r\n" + link;
                        System.Threading.Tasks.Task.Run(() => { _heeblo.SendEmail(user.email, subject, body); });
                        resp.RespObj = null;
                        resp.RespMsg = "Please verify your account first";
                        return resp;

                    }
                    if (!user.is_active)
                    {
                        resp.RespObj = null;
                        resp.RespMsg = "Your account is blocked";
                        return resp;

                    }
                    user.password = "dummy";
                    resp.Resp = true;
                    resp.RespObj = user;
                    resp.RespMsg = "Valid user";
                }
                else
                {
                    resp.RespObj = null;
                    resp.RespMsg = "Invalid credentials";
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
