using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required] // this build-in validation connected to [ApiController]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
