using Amazon.DynamoDBv2.DataModel;

namespace MagicVilla_VillaAPI.Data
{
    [DynamoDBTable("Villas")]
    public class VillaDB
    {
        [DynamoDBHashKey("Id")]
        public int Id {get; set;}
        [DynamoDBRangeKey("Name")]
        public string Name { get; set; }
    }
}
