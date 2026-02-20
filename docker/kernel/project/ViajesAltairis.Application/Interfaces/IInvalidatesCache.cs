namespace ViajesAltairis.Application.Interfaces;

public interface IInvalidatesCache
{
    static abstract IReadOnlyList<string> CachePrefixes { get; }
}
