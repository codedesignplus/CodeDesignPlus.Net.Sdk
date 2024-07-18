﻿namespace CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

public interface IProvider
{
    Task<Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default);
    Task<Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default);
    Task<Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default);
}
