using System.Collections.Generic;

namespace SlackNet.WebApi;

public class FunctionPermissionsResponse : SlackObject
{
    public FunctionPermissionType PermissionType { get; set; }
    public IList<FunctionPermissionUser> Users { get; set; }
}
