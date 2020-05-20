using System.IO;
using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SlackNet.Tests.Objects
{
    public class UserTests
    {
        [Test]
        public void Serialization_WorksCorrectlyForAvatarFields()
        {
            var jsonStr = @"
{
  ""id"": ""U12345678"",
  ""profile"": {
    ""image_24"": ""https://secure.gravatar.com/avatar/invalid.jpg?s=24\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-24.png"",
    ""image_32"": ""https://secure.gravatar.com/avatar/invalid.jpg?s=32\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-32.png"",
    ""image_48"": ""https://secure.gravatar.com/avatar/invalid.jpg?s=48\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-48.png"",
    ""image_72"": ""https://secure.gravatar.com/avatar/invalid.jpg?s=72\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-72.png"",
    ""image_192"": ""https://secure.gravatar.com/avatar/invalid.jpg?s=192\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-192.png"",
    ""image_512"": ""https://secure.gravatar.com/avatar/invalid.jpg?s=512\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-512.png""
  }
}";

            var serializer =
                JsonSerializer.Create(Default
                    .JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes))
                    .SerializerSettings);

            var user = serializer.Deserialize<User>(new JsonTextReader(new StringReader(jsonStr)));

            user.Profile.Image24.ShouldBe(
                "https://secure.gravatar.com/avatar/invalid.jpg?s=24\u0026d=https%3A%2F%2Fa.slack-edge.com%2Fdf10d%2Fimg%2Favatars%2Fava_0008-24.png");

        }
    }
}