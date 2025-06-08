using System.Diagnostics.CodeAnalysis;

namespace PingPal.Common.Extensions;

public enum ThreadLoading
{
    VeryHigh = 2,
    High = 4,
    Medium = 8,
    Low = 12,
    VeryLow = 16
}

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<TValue>([NotNullWhen(false)] this IEnumerable<TValue>? values)
    {
        return values == null || !values.Any();
    }

    public static bool IsSignificant<TValue>([NotNullWhen(true)] this IEnumerable<TValue>? values)
    {
        return values != null && values.Any();
    }

    public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> action)
    {
        foreach (var value in values)
            action(value);
    }

	public static Task WaitAllByPartsAsync<TIn>(this IEnumerable<TIn> collection, Func<TIn, Task> doAsync, ThreadLoading threadLoading)
    {
        return collection.WaitAllByPartsAsync(value => doAsync(value).ContinueWith(_ => false), threadLoading);
    }

    public static Task<TOut[]> WaitAllByPartsAsync<TIn, TOut>(this IEnumerable<TIn> collection, Func<TIn, Task<TOut>> doAsync, ThreadLoading threadLoading)
    {
        var count = (int) threadLoading;
        var semaphoreSlim = new SemaphoreSlim(count, count);
        return Task.WhenAll(
            collection.Select(
                value => Task.Run(
                    async () =>
                    {
                        await semaphoreSlim.WaitAsync();
                        try
                        {
                            return await doAsync(value);
                        }
                        finally
                        {
                            semaphoreSlim.Release();
                        }
                    })));
    }
}