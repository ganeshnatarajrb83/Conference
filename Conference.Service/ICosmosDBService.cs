using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conference.Domain;
namespace Conference.Service
{
    public interface ICosmosDBService
    {
        /// <summary>
        /// Gets session information from the conference supported by filter criteria
        /// </summary>
        /// <param name="speakername">speaker name</param>
        /// <param name="timeslot">date time slot</param>
        /// <returns>collection of sessions</returns>
        Task<ICollection<Collection>> GetSessionsAsync(string speakername, string timeslot);
    }

}
