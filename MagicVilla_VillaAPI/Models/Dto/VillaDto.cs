﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required] // this build-in validation connected to [ApiController]
        [MaxLength(30)]
        public string Name { get; set; }
        public Info Info { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
    }
}
