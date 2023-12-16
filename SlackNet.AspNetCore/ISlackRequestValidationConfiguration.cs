namespace SlackNet.AspNetCore;

public interface ISlackRequestValidationConfiguration
{
    bool VerifyEventUrl { get; }
    string SigningSecret { get; }
    string VerificationToken { get; }
}