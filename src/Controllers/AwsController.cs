using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TesteECS.Services;

namespace TesteECS.Controllers
{
    [Route("api/teste/aws")]
    public class AwsController
    {


        [HttpGet("sns/topics")]
        public async Task<IEnumerable<string>> ListTopics([FromServices] ISnsService service, CancellationToken cancellationToken)
            => await service.ListTopics(cancellationToken);

        public class CreateTopicRequest { public string Name { get; set; } }
        [HttpPost("sns/topics")]
        public async Task<string> CreateTopic([FromBody]CreateTopicRequest request, [FromServices] ISnsService service, CancellationToken cancellationToken)
            => await service.CreateTopic(request.Name, cancellationToken);

        public class SubscribeTopicRequest { public string QueueUrl { get; set; } }
        [HttpPost("sns/topics/{topicArn}/subscribers")]
        public async Task SubscribeTopic([FromRoute]string topicArn, [FromBody]SubscribeTopicRequest request, [FromServices] ISnsService service, CancellationToken cancellationToken)
            => await service.SubscribeTopic(topicArn, request.QueueUrl, cancellationToken);

        public class PublishTopicRequest { public dynamic Message { get; set; } }
        [HttpPost("sns/topics/{topicArn}/messages")]
        public async Task PublishTopic([FromRoute]string topicArn, [FromBody]PublishTopicRequest request, [FromServices] ISnsService service, CancellationToken cancellationToken)
            => await service.SubscribeTopic(topicArn, request.Message, cancellationToken);



        [HttpGet("sqs/queues")]
        public async Task<IEnumerable<string>> ListQueues([FromServices] ISqsService service, CancellationToken cancellationToken)
            => await service.ListQueues(cancellationToken);

        public class CreateQueueRequest { public string Name { get; set; } }
        [HttpPost("sqs/queues")]
        public async Task<string> CreateQueue([FromBody]CreateQueueRequest request, [FromServices] ISqsService service, CancellationToken cancellationToken)
            => await service.CreateQueue(request.Name, cancellationToken);




    }
}