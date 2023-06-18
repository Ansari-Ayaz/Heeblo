namespace Heeblo.Models
{
    public class UserDetailsWithUploadDocs
    {
        public int application_id { get; set; }
        public DateTime created_on { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string resume_filename { get; set; }
        public string sample_content_filename { get; set; }
        public string resume { get; set; }
        public string sample_content { get; set; }
    }
}
