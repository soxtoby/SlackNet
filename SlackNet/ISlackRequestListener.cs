namespace SlackNet
{
    public interface ISlackRequestListener
    {
        void OnRequestBegin(SlackRequestContext context);
    }
}