namespace CodeDesignPlus.Net.Mongo.Repository;

/// <summary>
/// Wraps a collection served by Azure DocumentDB so that every find without its own order is sorted by <c>_id</c>;
/// everything else goes to the wrapped collection untouched.
/// </summary>
/// <remarks>
/// DocumentDB returns, on an unsorted find that needs a <c>getMore</c> and filters an index on the fly, the last
/// documents of the first batch again (Kappali pendings/621). Any sort avoids it, and <c>_id</c> always exists and is
/// indexed. Every member of <see cref="IMongoCollection{TDocument}"/> is delegated one by one, with no default
/// implementation in between, so that writes, counts and aggregations reach the server exactly as without the wrapper.
/// </remarks>
/// <typeparam name="TDocument">The type of the document.</typeparam>
/// <param name="inner">The collection that receives every call.</param>
#pragma warning disable CS0618, CS0672 // The obsolete members of the driver are delegated as they are.
internal sealed class StableOrderCollection<TDocument>(IMongoCollection<TDocument> inner) : IMongoCollection<TDocument>
{
    /// <summary>
    /// The order added to a find that has none.
    /// </summary>
    internal static readonly SortDefinition<TDocument> IdAscending = Builders<TDocument>.Sort.Ascending("_id");

    /// <summary>
    /// Gets the collection that receives every call.
    /// </summary>
    internal IMongoCollection<TDocument> Inner => inner;

    /// <inheritdoc />
    public CollectionNamespace CollectionNamespace => inner.CollectionNamespace;

    /// <inheritdoc />
    public IMongoDatabase Database => inner.Database;

    /// <inheritdoc />
    public IBsonSerializer<TDocument> DocumentSerializer => inner.DocumentSerializer;

    /// <inheritdoc />
    public IMongoIndexManager<TDocument> Indexes => inner.Indexes;

    /// <inheritdoc />
    public IMongoSearchIndexManager SearchIndexes => inner.SearchIndexes;

    /// <inheritdoc />
    public MongoCollectionSettings Settings => inner.Settings;

    /// <summary>
    /// Sets <c>_id</c> ascending as the order of a find that brings none; a find with its own order is left as it is.
    /// </summary>
    /// <remarks>
    /// The options are completed in place: only an empty sort is filled, so reusing them gives the same order again.
    /// </remarks>
    internal static FindOptions<TDocument, TProjection> WithStableOrder<TProjection>(FindOptions<TDocument, TProjection> options)
    {
        options ??= new FindOptions<TDocument, TProjection>();

        options.Sort ??= IdAscending;

        return options;
    }

    /// <inheritdoc />
    public IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
    {
        return inner.FindSync(filter, WithStableOrder(options), cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncCursor<TProjection> FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
    {
        return inner.FindSync(session, filter, WithStableOrder(options), cancellationToken);
    }

    /// <inheritdoc />
    public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
    {
        return inner.FindAsync(filter, WithStableOrder(options), cancellationToken);
    }

    /// <inheritdoc />
    public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
    {
        return inner.FindAsync(session, filter, WithStableOrder(options), cancellationToken);
    }

    /// <inheritdoc />
    public IMongoCollection<TDocument> WithReadConcern(ReadConcern readConcern) => new StableOrderCollection<TDocument>(inner.WithReadConcern(readConcern));

    /// <inheritdoc />
    public IMongoCollection<TDocument> WithReadPreference(ReadPreference readPreference) => new StableOrderCollection<TDocument>(inner.WithReadPreference(readPreference));

    /// <inheritdoc />
    public IMongoCollection<TDocument> WithWriteConcern(WriteConcern writeConcern) => new StableOrderCollection<TDocument>(inner.WithWriteConcern(writeConcern));

    /// <inheritdoc />
    public IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>() where TDerivedDocument : TDocument => inner.OfType<TDerivedDocument>();

    /// <inheritdoc />
    public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.Aggregate(pipeline, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.Aggregate(session, pipeline, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.AggregateAsync(pipeline, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.AggregateAsync(session, pipeline, options, cancellationToken);

    /// <inheritdoc />
    public void AggregateToCollection<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.AggregateToCollection(pipeline, options, cancellationToken);

    /// <inheritdoc />
    public void AggregateToCollection<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.AggregateToCollection(session, pipeline, options, cancellationToken);

    /// <inheritdoc />
    public Task AggregateToCollectionAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.AggregateToCollectionAsync(pipeline, options, cancellationToken);

    /// <inheritdoc />
    public Task AggregateToCollectionAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => inner.AggregateToCollectionAsync(session, pipeline, options, cancellationToken);

    /// <inheritdoc />
    public BulkWriteResult<TDocument> BulkWrite(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => inner.BulkWrite(requests, options, cancellationToken);

    /// <inheritdoc />
    public BulkWriteResult<TDocument> BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => inner.BulkWrite(session, requests, options, cancellationToken);

    /// <inheritdoc />
    public Task<BulkWriteResult<TDocument>> BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => inner.BulkWriteAsync(requests, options, cancellationToken);

    /// <inheritdoc />
    public Task<BulkWriteResult<TDocument>> BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => inner.BulkWriteAsync(session, requests, options, cancellationToken);

    /// <inheritdoc />
    public long Count(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.Count(filter, options, cancellationToken);

    /// <inheritdoc />
    public long Count(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.Count(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<long> CountAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.CountAsync(filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<long> CountAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.CountAsync(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public long CountDocuments(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.CountDocuments(filter, options, cancellationToken);

    /// <inheritdoc />
    public long CountDocuments(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.CountDocuments(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<long> CountDocumentsAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.CountDocumentsAsync(filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<long> CountDocumentsAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default) => inner.CountDocumentsAsync(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public DeleteResult DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => inner.DeleteMany(filter, cancellationToken);

    /// <inheritdoc />
    public DeleteResult DeleteMany(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => inner.DeleteMany(filter, options, cancellationToken);

    /// <inheritdoc />
    public DeleteResult DeleteMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => inner.DeleteMany(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => inner.DeleteManyAsync(filter, cancellationToken);

    /// <inheritdoc />
    public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => inner.DeleteManyAsync(filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => inner.DeleteManyAsync(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public DeleteResult DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => inner.DeleteOne(filter, cancellationToken);

    /// <inheritdoc />
    public DeleteResult DeleteOne(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => inner.DeleteOne(filter, options, cancellationToken);

    /// <inheritdoc />
    public DeleteResult DeleteOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => inner.DeleteOne(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default) => inner.DeleteOneAsync(filter, cancellationToken);

    /// <inheritdoc />
    public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default) => inner.DeleteOneAsync(filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => inner.DeleteOneAsync(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TField> Distinct<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.Distinct(field, filter, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TField> Distinct<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.Distinct(session, field, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.DistinctAsync(field, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TField>> DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.DistinctAsync(session, field, filter, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TItem> DistinctMany<TItem>(FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.DistinctMany(field, filter, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TItem> DistinctMany<TItem>(IClientSessionHandle session, FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.DistinctMany(session, field, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.DistinctManyAsync(field, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(IClientSessionHandle session, FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => inner.DistinctManyAsync(session, field, filter, options, cancellationToken);

    /// <inheritdoc />
    public long EstimatedDocumentCount(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default) => inner.EstimatedDocumentCount(options, cancellationToken);

    /// <inheritdoc />
    public Task<long> EstimatedDocumentCountAsync(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default) => inner.EstimatedDocumentCountAsync(options, cancellationToken);

    /// <inheritdoc />
    public TProjection FindOneAndDelete<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndDelete(filter, options, cancellationToken);

    /// <inheritdoc />
    public TProjection FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndDelete(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndDeleteAsync(filter, options, cancellationToken);

    /// <inheritdoc />
    public Task<TProjection> FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndDeleteAsync(session, filter, options, cancellationToken);

    /// <inheritdoc />
    public TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndReplace(filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public TProjection FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndReplace(session, filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndReplaceAsync(filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public Task<TProjection> FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndReplaceAsync(session, filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public TProjection FindOneAndUpdate<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndUpdate(filter, update, options, cancellationToken);

    /// <inheritdoc />
    public TProjection FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndUpdate(session, filter, update, options, cancellationToken);

    /// <inheritdoc />
    public Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndUpdateAsync(filter, update, options, cancellationToken);

    /// <inheritdoc />
    public Task<TProjection> FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default) => inner.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken);

    /// <inheritdoc />
    public void InsertOne(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => inner.InsertOne(document, options, cancellationToken);

    /// <inheritdoc />
    public void InsertOne(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => inner.InsertOne(session, document, options, cancellationToken);

    /// <inheritdoc />
    public Task InsertOneAsync(TDocument document, CancellationToken _cancellationToken) => inner.InsertOneAsync(document, _cancellationToken);

    /// <inheritdoc />
    public Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => inner.InsertOneAsync(document, options, cancellationToken);

    /// <inheritdoc />
    public Task InsertOneAsync(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => inner.InsertOneAsync(session, document, options, cancellationToken);

    /// <inheritdoc />
    public void InsertMany(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => inner.InsertMany(documents, options, cancellationToken);

    /// <inheritdoc />
    public void InsertMany(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => inner.InsertMany(session, documents, options, cancellationToken);

    /// <inheritdoc />
    public Task InsertManyAsync(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => inner.InsertManyAsync(documents, options, cancellationToken);

    /// <inheritdoc />
    public Task InsertManyAsync(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => inner.InsertManyAsync(session, documents, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default) => inner.MapReduce(map, reduce, options, cancellationToken);

    /// <inheritdoc />
    public IAsyncCursor<TResult> MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default) => inner.MapReduce(session, map, reduce, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default) => inner.MapReduceAsync(map, reduce, options, cancellationToken);

    /// <inheritdoc />
    public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default) => inner.MapReduceAsync(session, map, reduce, options, cancellationToken);

    /// <inheritdoc />
    public ReplaceOneResult ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => inner.ReplaceOne(filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public ReplaceOneResult ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => inner.ReplaceOne(filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => inner.ReplaceOne(session, filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => inner.ReplaceOne(session, filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => inner.ReplaceOneAsync(filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => inner.ReplaceOneAsync(filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => inner.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken = default) => inner.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);

    /// <inheritdoc />
    public UpdateResult UpdateMany(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateMany(filter, update, options, cancellationToken);

    /// <inheritdoc />
    public UpdateResult UpdateMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateMany(session, filter, update, options, cancellationToken);

    /// <inheritdoc />
    public Task<UpdateResult> UpdateManyAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateManyAsync(filter, update, options, cancellationToken);

    /// <inheritdoc />
    public Task<UpdateResult> UpdateManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateManyAsync(session, filter, update, options, cancellationToken);

    /// <inheritdoc />
    public UpdateResult UpdateOne(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateOne(filter, update, options, cancellationToken);

    /// <inheritdoc />
    public UpdateResult UpdateOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateOne(session, filter, update, options, cancellationToken);

    /// <inheritdoc />
    public Task<UpdateResult> UpdateOneAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateOneAsync(filter, update, options, cancellationToken);

    /// <inheritdoc />
    public Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => inner.UpdateOneAsync(session, filter, update, options, cancellationToken);

    /// <inheritdoc />
    public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => inner.Watch(pipeline, options, cancellationToken);

    /// <inheritdoc />
    public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => inner.Watch(session, pipeline, options, cancellationToken);

    /// <inheritdoc />
    public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => inner.WatchAsync(pipeline, options, cancellationToken);

    /// <inheritdoc />
    public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => inner.WatchAsync(session, pipeline, options, cancellationToken);
}
#pragma warning restore CS0618, CS0672
