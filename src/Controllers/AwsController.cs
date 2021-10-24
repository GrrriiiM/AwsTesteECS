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



    }
}