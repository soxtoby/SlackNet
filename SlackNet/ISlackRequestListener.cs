#nullable disable
namespace SlackNet;

public interface ISlackRequestListener
{
    void OnRequestBegin(SlackRequestContext context);
}