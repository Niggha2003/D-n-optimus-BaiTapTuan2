using System.ComponentModel.DataAnnotations;

namespace BaiTap2.Models
{
    public class AuthorModel
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(200)]
        public string AuthorName { get; set; }

        public List<BookModel> Books { get; set; } = new List<BookModel>();   // cho bảng sách
    }
}



