using System;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Memory;

public interface IMemoryService<T> where T : class
{
    T? GetMemory(string key);
    void AddMemory(string key, T value);
}
