using System.ComponentModel.DataAnnotations;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Models
{
    public class Villa
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        public string Info { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
