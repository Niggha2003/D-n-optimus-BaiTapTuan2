using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaiTap2.Models.MongoDBModels
{
    public class BookReviewMongoDBModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReviewId { get; set; }
        public string ReviewContent { get; set; }
        public int ReviewerId { get; set; }  // khoá ngoại đến bảng reviewer
        public int BookId { get; set; }  // khoá ngoại đến bảng book
    }
}
