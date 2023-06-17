using System.ComponentModel.DataAnnotations;

namespace Heeblo.Models
{
    public class hbl_tbl_project
    {
        [Key]
        public int pid { get; set; }
        public string name { get; set; }
        public string? link { get; set; }
        public DateTime date { get; set; }
        public bool is_active { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime updated_on { get; set; }
        public int updated_by { get; set; }
    }
}
