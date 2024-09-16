using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Elysium.Core.Models;
using Elysium.Grains.Services;
using Elysium.GrainInterfaces.Services;

namespace Elysium.ActivityPub.Api.Controllers
{


    [Route("actors")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public ActorController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{actorName}")]
        public async Task<IActionResult> GetActorDocument(string actorName)
        {
            // Construct the IRI using IriBuilder and information from HttpContext
            var iriBuilder = new IriBuilder
            {
                Host = HttpContext.Request.Host.Host,
                Scheme = HttpContext.Request.Scheme,
                Path = $"actors/{actorName}"
            };

            // Construct the IRI
            Iri iri = iriBuilder.Iri;

            // todo:  use remote actor
            var author = NoSignatureAuthor.Instance;

            // Fetch the document from the document service
            var result = await _documentService.GetDocumentAsync(author, iri);

            if (result.IsSuccessful)
                // Return the document as JSON-LD with the proper content type
                return new ContentResult
                {
                    Content = result.Value.ToString(),
                    ContentType = "application/ld+json",
                    StatusCode = 200
                };
            else
                // Handle errors by returning the appropriate status code
                return StatusCode(500, "Failed to retrieve document.");
        }
    }
}
