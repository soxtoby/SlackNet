using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public class ResolvedDialogSubmissionHandler<T> : IDialogSubmissionHandler
        where T : IDialogSubmissionHandler
    {
        private readonly IServiceProvider _serviceProvider;
        public ResolvedDialogSubmissionHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                return await handler.Handle(dialog).ConfigureAwait(false);
            }
        }

        public async Task HandleCancel(DialogCancellation cancellation)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<T>();
                await handler.HandleCancel(cancellation).ConfigureAwait(false);
            }
        }
    }
}