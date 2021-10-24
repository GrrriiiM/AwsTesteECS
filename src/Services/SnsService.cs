using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using TesteECS.Settings;

namespace TesteECS.Services
{
    public class SnsService : ISnsService
    {
        readonly AwsSnsSettings _settings;
        readonly IAmazonSimpleNotificationService _client;
        readonly IAmazonSQS _sqsClient;

        public SnsService(IOptions<AwsSnsSettings> settings,
            IAmazonSimpleNotificationService client,
            IAmazonSQS sqsClient)
        {
            _settings = settings.Value;
            _client = client;
            _sqsClient = sqsClient;
        }

        public async Task<IEnumerable<string>> ListTopics(CancellationToken cancellationToken)
        {
            var request = new ListTopicsRequest();
            var response = new ListTopicsResponse();
            var topicsArn = new List<string>();

            do
            {
                response = await _client.ListTopicsAsync(request, cancellationToken);
                topicsArn.AddRange(response.Topics.Select(_ => _.TopicArn));
                request.NextToken = response.NextToken;

            } while (!string.IsNullOrEmpty(response.NextToken));
            return topicsArn;
        }

        public async Task<string> CreateTopic(string name, CancellationToken cancellationToken)
        {
            return (await _client.CreateTopicAsync(new CreateTopicRequest
            {
                Name = name
            }, cancellationToken)).TopicArn;
        }

        public async Task SubscribeTopic(string topicArn, string queueArn, CancellationToken cancellationToken)
        {
            await _client.SubscribeQueueToTopicsAsync(new[] { topicArn }, _sqsClient, queueArn);
        }

        public async Task<string> Publish(string topicArn, dynamic message, CancellationToken cancellationToken)
        {
            return (await _client.PublishAsync(new PublishRequest
            {
                TopicArn = topicArn,
                Message = System.Text.Json.JsonSerializer.Serialize(message)
            })).SequenceNumber;
        }
    }

    [Injectable]
    public interface ISnsService
    {
        Task<string> CreateTopic(string name, CancellationToken cancellationToken);
        Task<IEnumerable<string>> ListTopics(CancellationToken cancellationToken);
        Task SubscribeTopic(string topicArn, string queueArn, CancellationToken cancellationToken);
        Task<string> Publish(string topicArn, dynamic message, CancellationToken cancellationToken);
    }
}