using System;
using System.Reactive;
using System.Reactive.Linq;
using EasyAssertions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace SlackNet.Tests;

public class UtilsTests
{
    private TestScheduler _scheduler;

    [SetUp]
    public void SetUp()
    {
        _scheduler = new TestScheduler();
    }

    [Test]
    public void ToDateTime_FromTimestampString_NullOrEmpty_ReturnsNull()
    {
        ((string)null).ToDateTime().ShouldBeNull();
        string.Empty.ToDateTime().ShouldBeNull();
    }

    [Test]
    public void ToDateTime_FromTimestampString_InvalidTimestamp_ReturnsNull()
    {
        "foo".ToDateTime().ShouldBeNull();
    }

    [Test]
    public void ToDateTime_FromTimestampString_ValidTimestamp_ReturnsDateTime()
    {
        "42".ToDateTime().ShouldBe(new DateTime(1970, 1, 1, 0, 0, 42));
        "-42".ToDateTime().ShouldBe(new DateTime(1969, 12, 31, 23, 59, 18));
    }

    [Test]
    public void ToDateTime_FromInteger_Zero_ReturnsNull()
    {
        0.ToDateTime().ShouldBeNull();
    }

    [Test]
    public void ToDateTime_FromInteger_NonZero_ReturnsDateTime()
    {
        42.ToDateTime().ShouldBe(new DateTime(1970, 1, 1, 0, 0, 42));
        (-42).ToDateTime().ShouldBe(new DateTime(1969, 12, 31, 23, 59, 18));
    }

    [Test]
    public void ToDateTime_FromDecimal_Zero_ReturnsNull()
    {
        0m.ToDateTime().ShouldBeNull();
    }

    [Test]
    public void ToDateTime_FromDecimal_NonZero_ReturnsDateTime()
    {
        42m.ToDateTime().ShouldBe(new DateTime(1970, 1, 1, 0, 0, 42));
        (-42m).ToDateTime().ShouldBe(new DateTime(1969, 12, 31, 23, 59, 18));
    }

    [Test]
    public void RetryWithDelay_NoError_PassesThrough()
    {
        var result = _scheduler.CreateObserver<int>();

        new[] { 1, 2, 3 }.ToObservable()
            .RetryWithDelay(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), _scheduler)
            .Subscribe(result);

        result.Messages.ShouldMatch(new[]
            {
                OnNext(0, 1),
                OnNext(0, 2),
                OnNext(0, 3),
                OnCompleted<int>(0)
            });
    }

    [Test]
    public void RetryWithDelay_Errors_RetriesForLongerAndLongerPeriodsThenCapsOutAtMaximum()
    {
        var initialSubscription = 100;
        var initialDelay = 50;
        var delayIncrease = 10;
        var maxDelay = initialDelay + delayIncrease * 2 + 5;
        int subscription1 = initialSubscription,
            subscription2 = subscription1 + initialDelay + 1,
            subscription3 = subscription2 + initialDelay + delayIncrease + 1,
            subscription4 = subscription3 + initialDelay + delayIncrease * 2 + 1,
            subscription5 = subscription4 + maxDelay + 1,
            subscription6 = subscription5 + maxDelay + 1;
        var source = _scheduler.CreateColdObservable(OnError<int>(0));

        _scheduler.Start(() => source.RetryWithDelay(TimeSpan.FromTicks(initialDelay), TimeSpan.FromTicks(delayIncrease), TimeSpan.FromTicks(maxDelay), _scheduler), 0, initialSubscription, subscription6 + 1);

        source.Subscriptions.ShouldMatch(new[]
            {
                new Subscription(subscription1, subscription1 + 1),
                new Subscription(subscription2, subscription2 + 1),
                new Subscription(subscription3, subscription3 + 1),
                new Subscription(subscription4, subscription4 + 1),
                new Subscription(subscription5, subscription5 + 1),
                new Subscription(subscription6, subscription6 + 1)
            });
    }

    [Test]
    public void RetryWithDelay_ResetsDelayAfterSuccessfulValue()
    {
        var initialDelay = 100;
        var delayIncrease = 10;
        var disposed = 2000;
        int subscription1 = 1,
            subscription2 = subscription1 + initialDelay + 1,
            subscription3 = subscription2 + initialDelay + delayIncrease + 1,
            subscription4 = subscription3 + initialDelay + 1;
        var source = _scheduler.CreateHotObservable(
            OnError<int>(subscription1 + 1),
            OnError<int>(subscription2 + 1),
            OnNext(subscription3 + 1, 0),
            OnError<int>(subscription3 + 1));

        _scheduler.Start(() => source.RetryWithDelay(TimeSpan.FromTicks(initialDelay), TimeSpan.FromTicks(delayIncrease), TimeSpan.FromTicks(200), _scheduler), 0, 0, disposed);

        source.Subscriptions.ShouldMatch(new[]
            {
                new Subscription(subscription1, subscription1 + 1),
                new Subscription(subscription2, subscription2 + 1),
                new Subscription(subscription3, subscription3 + 1),
                new Subscription(subscription4, disposed)
            });
    }

    [Test]
    public void RetryWithDelay_DoesNotStackOverflow()
    {
        var source = Observable.Throw<int>(new Exception());

        _scheduler.Start(() => source.RetryWithDelay(TimeSpan.FromTicks(1), TimeSpan.FromTicks(0), TimeSpan.FromTicks(1), _scheduler), 0, 0, 10000);
    }

    private static Recorded<Notification<T>> OnNext<T>(long time, T value) => new(time, Notification.CreateOnNext(value));
    private static Recorded<Notification<T>> OnError<T>(long time) => new(time, Notification.CreateOnError<T>(new Exception()));
    private static Recorded<Notification<T>> OnCompleted<T>(long time) => new(time, Notification.CreateOnCompleted<T>());
}