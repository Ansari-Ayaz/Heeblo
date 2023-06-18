using System.ComponentModel.DataAnnotations;

namespace Heeblo.Models
{
    public class hbl_tbl_attachment
    {
        [Key]
        public int attachment_id { get; set; }

        public int application_id { get; set; }
        public string resume { get; set; }
        public string sample_content { get; set; }
        public string resume_filename { get; set; }
        public string sample_content_filename { get; set; }
    }
}