namespace SlackNet
{
    /// <summary>
    /// An installation of your app. Installations are defined by a combination of the installing Enterprise Grid org,
    /// workspace, and user (represented by <see cref="EnterpriseId"/>, <see cref="TeamId"/> and <see cref="UserId"/> inside this class)
    /// Installations may only have one or two, not all three, defined.
    /// </summary>
    public class Authorization
    {
        public string EnterpriseId { get; set; }

        public string TeamId { get; set; }

        public string UserId { get; set; }
        
        public bool IsBot { get; set; }
            
    }
}