using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using Newtonsoft.Json;

namespace SqsWriter.Integration.Test.Tests
{
    [TestClass]
    public class PublishTest : BaseTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SqsClientMock.Setup(x => x.GetMessagesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Message>());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SqsClientMock.Verify(x => x.CreateQueueAsync(), Times.Once);
            SqsClientMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task PublishMovie_SendSqsMessage()
        {
            var movie = new Movie { Title = "Die hard", Genre = MovieGenre.Action };
            var response = await PublishClient.PublishMovie(movie);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            SqsClientMock.Verify(x => x.PostMessageAsync(
                It.Is<Movie>(y => JsonConvert.SerializeObject(y) == JsonConvert.SerializeObject(movie))), Times.Once);
        }

        [TestMethod]
        public async Task PublishActor_SendSqsMessage()
        {
            var actor = new Actor { FirstName = "Bruce", LastName = "Willis" };
            var response = await PublishClient.PublishActor(actor);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            SqsClientMock.Verify(x => x.PostMessageAsync(
                It.Is<Actor>(y => JsonConvert.SerializeObject(y) == JsonConvert.SerializeObject(actor))), Times.Once);
        }
    }
}