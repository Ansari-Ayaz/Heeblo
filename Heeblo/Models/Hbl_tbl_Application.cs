using System.ComponentModel.DataAnnotations;

namespace Heeblo.Models
{
    public class hbl_tbl_application
    {
        [Key]
        public int application_id { get; set; }
        public int pid { get; set; }
        public int uid { get; set; }       
        public DateTime date { get; set; }
        public decimal ai_score { get; set; }
        public decimal grammar_score { get; set; }
        public decimal plagiarism { get; set; }
        public string? status { get; set; }
        public bool is_active { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime updated_on { get; set; }
        public int updated_by { get; set; }
    }
    public class application_view
    {
        public int pid { get; set; }
        public int uid { get; set; }

        public IFormFile resume { get; set; }
        public IFormFile sample_content { get; set; }
        public int created_by { get; set; }
    }
}
