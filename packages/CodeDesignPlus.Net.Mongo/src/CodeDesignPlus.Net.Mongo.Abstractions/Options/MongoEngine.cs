namespace CodeDesignPlus.Net.Mongo.Abstractions.Options;

/// <summary>
/// Identifies the server that answers the MongoDB wire protocol, so the library can work around the defects of an
/// engine that is compatible with MongoDB but is not MongoDB.
/// </summary>
public enum MongoEngine
{
    /// <summary>
    /// MongoDB itself (self-hosted or MongoDB Atlas). The library sends the queries exactly as the caller built them.
    /// </summary>
    MongoDb = 0,

    /// <summary>
    /// Azure DocumentDB (PostgreSQL with pg_documentdb). An unsorted find that needs more than one batch can return
    /// documents of the first batch again, so every find that does not bring its own order is sorted by <c>_id</c>.
    /// </summary>
    AzureDocumentDb = 1
}
