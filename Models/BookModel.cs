using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaiTap2.Models
{
    public class BookModel
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public int AuthorId { get; set; }  // khoá ngoại đến bảng author
        public AuthorModel Author { get; set; }

        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }
}
