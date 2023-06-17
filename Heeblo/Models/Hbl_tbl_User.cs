using System.ComponentModel.DataAnnotations;

namespace Heeblo.Models
{
	public class hbl_tbl_user
	{
		[Key]
		public int uid { get; set; }
		public string name { get; set; }
		public string mobile { get; set; }
		public string email { get; set; }
		public string password { get; set; }
		public int role { get; set; }
		public bool verified { get; set; }
		public bool is_active { get; set; }
		public DateTime created_on { get; set; }
		public int created_by { get; set; }
		public DateTime update_on { get; set; }
		public int updated_by { get; set; }

	}
}
