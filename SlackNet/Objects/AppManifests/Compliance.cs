#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// Compliance certifications for GovSlack.
/// </summary>
public class Compliance
{
    /// <summary>
    /// FedRAMP certification.
    /// </summary>
    public string? FedrampAuthorization { get; set; }

    /// <summary>
    /// Department of Defense (DoD) Cloud Computing Security Requirements Guide (SRG).
    /// </summary>
    public string? DodSrgIlx { get; set; }

    /// <summary>
    /// ITAR compliance.
    /// </summary>
    public string? ItarCompliant { get; set; }
}
