using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace IntegrationTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task MasterResponseIsCorrect()
        {
            var client = new HttpClient();

            var response = await client.GetAsync("http://master/api/greetings");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().NotBeNull();
            responseString.Should().Contain("Hello Lord! I am your Master");
            true.Should().BeFalse();
        }

        [Fact]
        public async Task SlaveResponseIsCorrect()
        {
            var client = new HttpClient();

            var response = await client.GetAsync("http://slave/api/identification");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().NotBeNull();
            responseString.Should().Contain("I am poor slave called");
        }

        [Fact]
        public async Task MasterSlaveConnectionWorks()
        {
            var client = new HttpClient();

            var response = await client.GetAsync("http://master/api/greetings");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().NotBeNull();
            responseString.Should().Contain("Hello Lord! I am your Master");
            Assert.Contains("I am poor slave called", responseString);
        }
    }
}
