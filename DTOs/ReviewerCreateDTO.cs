using BaiTap2.Models;
using System.ComponentModel.DataAnnotations;

namespace BaiTap2.DTOs
{
    public class ReviewerCreateDTO
    {
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; }
    }
}
