using Amazon.DynamoDBv2.DataModel;

namespace MagicVilla_VillaAPI.Models
{
    [DynamoDBTable("Villas")]
    public class VillaDB
    {
        [DynamoDBHashKey("Id")]
        public int Id {get; set;}
        //[DynamoDBRangeKey("Name")]
        public string Name { get; set; }
        public string Body { get; set; }
        //public int Sqft { get; set; }
    }
}
