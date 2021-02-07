using Conference.Domain;
using Conference.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conference.Controllers
{
    /// <summary>
    /// Conference controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConferenceController : ControllerBase
    {
        private readonly ICosmosDBService cosmosDBService;
        
        public ConferenceController(ICosmosDBService cosmosDBService)
        {
            this.cosmosDBService = cosmosDBService;
        }

        /// <summary>
        /// A list of sessions. Optional parameters work as filters to reduce the listed sessions.
        /// </summary>
        /// <param name="speakername">speaker name</param>
        /// <param name="timeslot">date time slot</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ICollection<Collection>), 200)]
        [ProducesResponseType(204)]
        [Route("sessions")]
        [HttpGet]
        public async Task<ICollection<Collection>> GetSessionsAsync(string speakername, string timeslot)
        {
            return await cosmosDBService.GetSessionsAsync(speakername, timeslot);
        }

    }
}
