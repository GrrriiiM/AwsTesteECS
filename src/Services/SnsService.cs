using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using TesteECS.Settings;

namespace TesteECS.Services
{
    public class SnsService : ISnsService
    {
        readonly AwsSnsSettings _settings;
        readonly IAmazonSimpleNotificationService _client;

        public SnsService(IOptions<AwsSnsSettings> settings,
            IAmazonSimpleNotificationService client)
        {
            _settings = settings.Value;
            _client = client;
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

        public Task Emit(dynamic message)
        {
            return Task.CompletedTask;
        }
    }

    public interface ISnsService
    {
        Task<string> CreateTopic(string name, CancellationToken cancellationToken);
        Task<IEnumerable<string>> ListTopics(CancellationToken cancellationToken);
        Task Emit(dynamic message);
    }
}