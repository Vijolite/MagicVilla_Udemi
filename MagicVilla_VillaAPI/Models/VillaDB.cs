using Amazon.DynamoDBv2.DataModel;
using MagicVilla_VillaAPI.Models.Dto;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Models
{
    [DynamoDBTable("Villas")]
    public class VillaDB
    {
        [DynamoDBHashKey("Id")]
        public int Id {get; set;}
        public string Name { get; set; }
        public string Body { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public VillaDB (VillaDto villaDto)
        {
            Id = villaDto.Id;
            Name = villaDto.Name;
            Body = JsonSerializer.Serialize(villaDto.Info);
            CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
            UpdatedDate = null;
        }
        public VillaDB ()
        {

        }
    }
}
