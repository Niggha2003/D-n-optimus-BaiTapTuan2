using System.ComponentModel.DataAnnotations;

namespace BaiTap2.DTOs
{
    public class ReviewCreateDTO
    {
        public int ReviewId { get; set; }
        public string ReviewContent { get; set; }
        public int BookId { get; set; }   // khoá ngoại đến bảng book
        public int ReviewerId { get; set; }  // khoá ngoại đến bảng reviewer
    }
}
