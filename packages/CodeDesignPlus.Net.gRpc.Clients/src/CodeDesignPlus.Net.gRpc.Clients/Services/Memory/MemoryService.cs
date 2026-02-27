using System;
using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Memory;

internal class MemoryService<T> : IMemoryService<T>
    where T : class
{
    private readonly ConcurrentDictionary<string, T?> memory = new();

    public T? GetMemory(string key)
    {
        var item = memory.TryGetValue(key, out var value) ? value : null;

        return item;
    }

    public void AddMemory(string key, T value)
    {
        memory[key] = value;
    }
}
