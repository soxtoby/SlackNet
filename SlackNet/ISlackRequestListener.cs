namespace SlackNet
{
    public interface ISlackRequestListener
    {
        void OnRequestBegin(SlackRequestContext context);
    }

    class NullRequestListener : ISlackRequestListener
    {
        private NullRequestListener() { }
        public static readonly ISlackRequestListener Instance = new NullRequestListener();

        public void OnRequestBegin(SlackRequestContext context) { }
    }
}