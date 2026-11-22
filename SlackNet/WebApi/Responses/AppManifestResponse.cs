using SlackNet.AppManifests;

namespace SlackNet.WebApi;

class AppManifestResponse
{
    public required AppManifest Manifest { get; set; }
}
