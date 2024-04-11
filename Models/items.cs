using System.ComponentModel.DataAnnotations;

namespace Supershop.Models
{
    public class items
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        public int? Count { get; set; }
        = 0;
        [Range(0, maximum: 1000)]
        public string location { get; set; }
        [Required]
        public double Price { get; set; }
        public string? ImageUrl { get; set; }

    }
}
