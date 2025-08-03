using BaiTap2.Models;
using System.ComponentModel.DataAnnotations;

namespace BaiTap2.DTOs
{
    public class BookCreateDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }  // khoá ngoại đến bảng author
    }
}
