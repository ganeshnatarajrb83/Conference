using Microsoft.Azure.Cosmos;
using Moq;
using System.Threading;
using Xunit;
using Conference.Service;
using Conference.Domain;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Conference.Test
{
    public class ConferenceTest
    {

        private Mock<CosmosClient> cosmosClient;
        private Mock<DatabaseResponse> databaseResponse;
        private Mock<Container> mockContainer;

        public ConferenceTest()
        {
            mockContainer = new Mock<Container>();
            databaseResponse = new Mock<DatabaseResponse>();
            cosmosClient = new Mock<CosmosClient>();
        }

        /// <summary>
        /// Unit test for getting sessions with results
        /// </summary>
        [Fact]
        public void GetSessionsTestWithResults()
        {
            //Arrange
            string mockResponse = "[{'Items':{'href':'https://conferenceapi.azurewebsites.net/session/101','data':[{'name':'Title','value':'\r\n\t\t\tAsync in C# 5\r\n\t\t'},{'name':'Timeslot','value':'04 December 2013 10:20 - 11:20'},{'name':'Speaker','value':'Jon Skeet'}],'links':[{'rel':'http://tavis.net/rels/speaker','href':'https://conferenceapi.azurewebsites.net/speaker/6'},{'rel':'http://tavis.net/rels/topics','href':'https://conferenceapi.azurewebsites.net/session/101/topics'}]}},{'Items':{'href':'https://conferenceapi.azurewebsites.net/session/127','data':[{'name':'Title','value':'\r\n\t\t\tSemantics matter\r\n\t\t'},{'name':'Timeslot','value':'04 December 2013 15:00 - 16:00'},{'name':'Speaker','value':'Jon Skeet'}],'links':[{'rel':'http://tavis.net/rels/speaker','href':'https://conferenceapi.azurewebsites.net/speaker/6'},{'rel':'http://tavis.net/rels/topics','href':'https://conferenceapi.azurewebsites.net/session/127/topics'}]}},{'Items':{'href':'https://conferenceapi.azurewebsites.net/session/133','data':[{'name':'Title','value':'\r\n\t\t\tLearning from Noda Time: a case study in API design and open source (good, bad and ugly)\r\n\t\t'},{'name':'Timeslot','value':'04 December 2013 16:20 - 17:20'},{'name':'Speaker','value':'Jon Skeet'}],'links':[{'rel':'http://tavis.net/rels/speaker','href':'https://conferenceapi.azurewebsites.net/speaker/6'},{'rel':'http://tavis.net/rels/topics','href':'https://conferenceapi.azurewebsites.net/session/133/topics'}]}}]";
            var mockCollectionResponse = JsonConvert.DeserializeObject<ICollection<Collection>> (mockResponse);
            var collectionList = new List<Collection>();
            collectionList.AddRange(mockCollectionResponse);

            this.cosmosClient.Setup(d => d.CreateDatabaseIfNotExistsAsync("confdb", It.IsAny<int>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(databaseResponse.Object);
            cosmosClient.Setup(x=> x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var feedIteratorMock = new Mock<FeedIterator<Collection>>();
            feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);

            var feedResponseMock = new Mock<FeedResponse<Collection>>();
            feedResponseMock.Setup(x => x.GetEnumerator()).Returns(collectionList.GetEnumerator());

            feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));

            mockContainer
                .Setup(c => c.GetItemQueryIterator<Collection>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            //Act
            var cosmosService = new CosmosDBService(cosmosClient.Object, "confdb", "ConferenceSession");
            var serviceResponse = cosmosService.GetSessionsAsync(string.Empty, string.Empty).Result;

            //Assert
            Assert.Equal(mockCollectionResponse.Count, serviceResponse.Count);

        }

        /// <summary>
        /// Unit test for getting sessions with no results are returned
        /// </summary>
        [Fact]
        public void GetSessionsTestNoResults()
        {
            //Arrange
            var mockCollectionResponse = new List<Collection>();
            var collectionList = new List<Collection>();

            this.cosmosClient.Setup(d => d.CreateDatabaseIfNotExistsAsync("confdb", It.IsAny<int>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(databaseResponse.Object);
            cosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(mockContainer.Object);

            var feedIteratorMock = new Mock<FeedIterator<Collection>>();
            feedIteratorMock.Setup(f => f.HasMoreResults).Returns(true);

            var feedResponseMock = new Mock<FeedResponse<Collection>>();
            feedResponseMock.Setup(x => x.GetEnumerator()).Returns(collectionList.GetEnumerator());

            feedIteratorMock
                .Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object)
                .Callback(() => feedIteratorMock
                    .Setup(f => f.HasMoreResults)
                    .Returns(false));

            mockContainer
                .Setup(c => c.GetItemQueryIterator<Collection>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            //Act
            var cosmosService = new CosmosDBService(cosmosClient.Object, "confdb", "ConferenceSession");
            var serviceResponse = cosmosService.GetSessionsAsync(string.Empty, string.Empty).Result;

            //Assert
            Assert.Equal(0, serviceResponse.Count);

        }
    }
}
