using DataModels.API;
using EventsAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventsAPI.Controllers
{
    [Route("v{version:apiVersion}/api/[controller]")]

    [ApiController]
    [ApiVersion("1.0")]
    public class ScansController : ControllerBase
    {
        private readonly ILogger<ScansController> _logger;
        public ScansController(ILogger<ScansController> logger)
        {
            _logger = logger;
        }


        [HttpGet("scanevents")]
        [SwaggerOperation(Summary = "Retrieve Scan Events")]
        public EventCollection? GetScanEvents(ulong fromEventId=1, int limit=100)
        {
            try
            {
                return new EventsDataRepository().GetEvents(fromEventId, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

    }
}
