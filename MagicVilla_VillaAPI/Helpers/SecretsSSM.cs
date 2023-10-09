using System.Data.Common;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json;

namespace MagicVilla_VillaAPI.Helpers
{
    public static class SecretsSSM
    {
        public static string GetSecretUrl()
        {
            var request = new GetParameterRequest()
            {
                Name = "NewAddedVillasSQSUrl"
            };
            using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName("us-east-1")))
            {
                var response = client.GetParameterAsync(request).GetAwaiter().GetResult();
                var connectionString = response.Parameter.Value;
                return connectionString;
            }
        }
    }
}
