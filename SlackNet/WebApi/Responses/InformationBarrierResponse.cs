#nullable enable
namespace SlackNet.WebApi;

record InformationBarrierResponse
{
    public required InformationBarrier Barrier { get; init; }
}
