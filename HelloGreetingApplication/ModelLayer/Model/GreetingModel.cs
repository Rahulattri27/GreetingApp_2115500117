using System.ComponentModel.DataAnnotations;
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
    }
}
