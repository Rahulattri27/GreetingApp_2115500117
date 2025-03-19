using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ModelLayer.Model
{
    /// <summary>
    /// Greeting Model for Greeting from user and database schema
    /// </summary>
    public class GreetingModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; } = "";

        [Required]
        public int UserId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
