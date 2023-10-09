using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.Runtime;
using System.Runtime.CompilerServices;

namespace MagicVilla_VillaAPI.Helpers
{
    public static class VillaSQS
    {

        // Method to put a message on a queue
        // Could be expanded to include message attributes, etc., in a SendMessageRequest
        private static async Task SendMessage(IAmazonSQS sqsClient, string qUrl, string messageBody)
        {
            SendMessageResponse responseSendMsg = await sqsClient.SendMessageAsync(qUrl, messageBody);
            Console.WriteLine($"Message added to queue\n  {qUrl}");
            Console.WriteLine($"HttpStatusCode: {responseSendMsg.HttpStatusCode}");
        }

        public static async Task SendMassageToNewAddedVillasSQS(string message)
        {
            var sqsClient = new AmazonSQSClient();
            string NewAddedVillasSQSUrl = SecretsSSM.GetSecretUrl();
            await SendMessage(sqsClient, NewAddedVillasSQSUrl, message);
        }
    }
}

