using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using TesteECS.Settings;

namespace TesteECS.Services
{
    public class SqsService : ISqsService
    {
        readonly AwsSnsSettings _settings;
        readonly IAmazonSQS _client;

        public SqsService(IOptions<AwsSnsSettings> settings,
            IAmazonSQS client)
        {
            _settings = settings.Value;
            _client = client;
        }

        // public async Task<IEnumerable<string>> ListTopics(CancellationToken cancellationToken)
        // {
        //     var request = new ListTopicsRequest();
        //     var response = new ListTopicsResponse();
        //     var topicsArn = new List<string>();

        //     do
        //     {
        //         response = await _client.ListTopicsAsync(request, cancellationToken);
        //         topicsArn.AddRange(response.Topics.Select(_ => _.TopicArn));
        //         request.NextToken = response.NextToken;

        //     } while (!string.IsNullOrEmpty(response.NextToken));
        //     return topicsArn;
        // }

        public async Task<string> CreateQueue(string name, CancellationToken cancellationToken)
        {
            return (await _client.CreateQueueAsync(new CreateQueueRequest
            {
                QueueName = name
            }, cancellationToken)).QueueUrl;
        }

        public async Task<IEnumerable<string>> ListQueues(CancellationToken cancellationToken)
        {
            var request = new ListQueuesRequest();
            var response = new ListQueuesResponse();
            var queuesUrls = new List<string>();

            do
            {
                response = await _client.ListQueuesAsync(request, cancellationToken);
                queuesUrls.AddRange(response.QueueUrls);
                request.NextToken = response.NextToken;

            } while (!string.IsNullOrEmpty(response.NextToken));
            return queuesUrls;
        }

        public async Task<string> ReceiveMessageQueue(string queueUrl, CancellationToken cancellationToken, int waitTime = 1)
        {
            var response = await _client.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = waitTime
            }, cancellationToken);
            if (response.Messages.Any())
            {
                var body = JsonSerializer.Deserialize<Dictionary<string, object>>(response.Messages.FirstOrDefault().Body);
                await _client.DeleteMessageAsync(queueUrl, response.Messages.FirstOrDefault().ReceiptHandle);
                return body["Message"].ToString();
            }
            return null;
        }
    }

    [Injectable]
    public interface ISqsService
    {
        Task<string> CreateQueue(string name, CancellationToken cancellationToken);
        Task<IEnumerable<string>> ListQueues(CancellationToken cancellationToken);
        Task<string> ReceiveMessageQueue(string queueUrl, CancellationToken cancellationToken, int waitTime = 1);
    }
}