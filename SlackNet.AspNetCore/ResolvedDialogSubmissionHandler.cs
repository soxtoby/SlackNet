using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedDialogSubmissionHandler : ResolvedHandler<IDialogSubmissionHandler>, IDialogSubmissionHandler
    {
        public ResolvedDialogSubmissionHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IDialogSubmissionHandler> getHandler) 
            : base(serviceProvider, getHandler) { }

        public Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog) => ResolvedHandle(h => h.Handle(dialog));

        public Task HandleCancel(DialogCancellation cancellation) => ResolvedHandle(h => h.HandleCancel(cancellation));
    }
}