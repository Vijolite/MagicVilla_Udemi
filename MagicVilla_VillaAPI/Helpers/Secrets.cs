using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace MagicVilla_VillaAPI.Helpers
{
    public static class Secrets
    {
        /*
        *	Use this code snippet in your app.
        *	If you need more information about configurations or implementing the sample code, visit the AWS docs:
        *	https://aws.amazon.com/developer/language/net/getting-started
        */
        public static async Task<string> GetSecret()
        {
            string secretName = "mysecrets";
            string region = "us-east-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                // For a list of the exceptions thrown, see
                // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
                throw e;
            }

            return response.SecretString;
        }

        public static async Task<string> GetOneSecretPairValue(string key)
        {
            var jsonSecrets = await Secrets.GetSecret();
            var secretDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonSecrets);
            return secretDict[key];
        }
    }
}
