using System;
using System.Threading.Tasks;

namespace SlackNet.Tests;

class RequestCompletionListener(Action completed) : ISlackRequestListener
{
    public void OnRequestBegin(SlackRequestContext context) => context.OnComplete(() =>
        {
            completed();
            return Task.CompletedTask;
        });
}
