
namespace System
{
    /// <summary>
    /// Defines scope for <see cref="IServiceProvider"/>.
    /// </summary>
    public interface IServiceProviderScope : IServiceProvider, IDisposable
    {
    }
}