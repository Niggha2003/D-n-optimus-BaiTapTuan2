using System.ComponentModel.DataAnnotations;

namespace BaiTap2.Models
{
    public class ReviewerModel
    {
        [Key]
        public int ReviewerId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ReviewerName { get; set; }

        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    }
}
