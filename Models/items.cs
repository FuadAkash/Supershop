using System.ComponentModel.DataAnnotations;

namespace Supershop.Models
{
    public class items
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; }
        
        [Required]
        public string Type { get; set; }
        public int? Count { get; set; }
        = 0;
        
        public string? location { get; set; }
        
        [Required]
        public double Price { get; set; }
        public string? ImageUrl { get; set; }

    }
}
