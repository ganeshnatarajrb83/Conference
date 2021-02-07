using Conference.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;

namespace Conference.Service
{
    public class CosmosDBService : ICosmosDBService
    {
        private static Container _container;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbClient">Azure cosmos db client</param>
        /// <param name="databaseName">Azure cosmos database name</param>
        /// <param name="containerName">Azure cosmos container name</param>
        public CosmosDBService(CosmosClient dbClient,
            string databaseName,
            string containerName
            )
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        /// <summary>
        /// Gets session information from the conference supported by filter criteria
        /// </summary>
        /// <param name="speakername">speaker name</param>
        /// <param name="timeslot">date time slot</param>
        /// <returns>collection of sessions</returns>
        public async Task<ICollection<Collection>> GetSessionsAsync(string speakername, string timeslot)
        {
            var iterator = _container.GetItemQueryIterator<Collection>(new QueryDefinition(GetQuery(speakername, timeslot)), null, null);
            List<Collection> itemsCollection = new List<Collection>();
            while (iterator.HasMoreResults)
            {
                var currentResultSet = await iterator.ReadNextAsync();
                foreach (var res in currentResultSet)
                {
                    var items = res;
                    itemsCollection.Add(items);
                }
            }

            return itemsCollection;
        }
        /// <summary>
        /// Gets the query associated with retrieval of information from Azure cosmos db
        /// </summary>
        /// <param name="speakername">speaker name</param>
        /// <param name="timeslot">date time slot</param>
        /// <returns></returns>
        private string GetQuery(string speakername, string timeslot)
        {
            string query = $"select itms as Items from c join c.collection as f join itms IN f.items";

            if (!string.IsNullOrEmpty(speakername) && !string.IsNullOrEmpty(timeslot))
                query = $"{query} join d IN itms.data join e IN itms.data where d['value']= '{timeslot}' AND e['value']= '{speakername}'";
            else if (!string.IsNullOrEmpty(timeslot))
                query = $"{query} join d IN itms.data where d['value']= '{timeslot}' ";
            else if (!string.IsNullOrEmpty(speakername))
                query = $"{query} join d IN itms.data where d['value']= '{speakername}' ";

            return query;
        }
    }
}
