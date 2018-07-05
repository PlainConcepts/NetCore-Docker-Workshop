using FluentAssertions;
using Master.Services;
using System;
using Xunit;

namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Guid_provider_should_not_return_an_empty_guid()
        {
            var provider = new GuidProvider();
            var id = provider.Id;
            id.Should().NotBe(Guid.Empty, "Guid should not be empty.");
        }
    }
}
