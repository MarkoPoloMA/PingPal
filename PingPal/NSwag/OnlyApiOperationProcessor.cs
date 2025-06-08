using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace PingPal.NSwag;

public class OnlyApiOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        return context.OperationDescription.Path
            .StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
    }
}