namespace Heeblo.Models
{
    public class AllApplication
    {
        public int application_id { get; set; }
        public DateTime created_on { get; set; }
        public string created_by { get; set; }
        public decimal ai_score { get; set; }
        public decimal grammar_score { get; set; }
        public decimal  plagiarism { get; set; }
        public string status { get; set; }
    }
}
