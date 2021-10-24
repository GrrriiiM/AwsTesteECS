using System.Collections.Generic;
using System.Linq;
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
    }

    [Injectable]
    public interface ISqsService
    {
        Task<string> CreateQueue(string name, CancellationToken cancellationToken);
        Task<IEnumerable<string>> ListQueues(CancellationToken cancellationToken);
    }
}