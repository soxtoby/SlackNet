using EasyAssertions;
using NUnit.Framework;

namespace SlackNet.Tests;

[SetUpFixture]
public class AssemblySetup
{
    [OneTimeSetUp]
    public void SetUp()
    {
        EasyAssertion.UseFrameworkExceptions(m => new AssertionException(m), (m, e) => new AssertionException(m, e));
    }
}