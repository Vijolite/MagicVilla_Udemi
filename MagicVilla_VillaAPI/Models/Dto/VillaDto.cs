using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        [Required] // this build-in validation connected to [ApiController]
        [MaxLength(30)]
        public string Name { get; set; }
        public Info Info { get; set; }

        public VillaDto (VillaDB villa)
        {
            Id = villa.Id;  
            Name = villa.Name;
            Info = JsonSerializer.Deserialize<Info>(villa.Body);
        }
        public VillaDto()
        {

        }

    }
}
