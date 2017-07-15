namespace SlackNet.WebApi
{
    public enum ProfileFieldVisibility
    {
        /// <summary>
        /// Return all fields.
        /// </summary>
        All,
        /// <summary>
        /// Return only fields for which the <see cref="TeamProfileField.IsHidden"/> option is False.
        /// </summary>
        Visible,
        /// <summary>
        /// Return only fields for which the <see cref="TeamProfileField.IsHidden"/> option is True.
        /// </summary>
        Hidden
    }
}