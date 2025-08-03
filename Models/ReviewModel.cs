using System.ComponentModel.DataAnnotations;

namespace BaiTap2.Models
{
    public class ReviewModel
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ReviewContent { get; set; }

        [Required]
        public int BookId { get; set; }   // khoá ngoại đến bảng book
        public BookModel Book { get; set; }

        [Required]
        public int ReviewerId { get; set; }  // khoá ngoại đến bảng reviewer
        public ReviewerModel Reviewer { get; set; }     
    }
}
