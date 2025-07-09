namespace CodeDesignPlus.Net.EFCore.Repository;

/// <summary>
/// This abstract class implements the most common methods for interacting with the database.
/// </summary>
/// <remarks>
/// Create a new instance of RepositoryBase.
/// </remarks>
/// <param name="context">Represents a session with the database and can be used to query and save instances of your entities.</param>
/// <exception cref="ArgumentNullException">Thrown if context is null.</exception>
public abstract class RepositoryBase(DbContext context) : IRepositoryBase
{
    /// <summary>
    /// Represents a session with the database and can be used to query and save instances of your entities.
    /// </summary>
    protected readonly DbContext Context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Converts the DbContext to the specified generic type.
    /// </summary>
    /// <typeparam name="TContext">Type of context to return.</typeparam>
    /// <returns>Returns the context of the database.</returns>
    public TContext GetContext<TContext>() where TContext : DbContext => (TContext)this.Context;

    /// <summary>
    /// Gets a DbSet that can be used to query and save instances of TEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    public DbSet<TEntity> GetEntity<TEntity>() where TEntity : class, IEntityBase => this.Context.Set<TEntity>();

    /// <summary>
    /// Creates an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create.</typeparam>
    /// <param name="entity">Entity to create.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.ProcessCreateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Processes the creation of an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create.</typeparam>
    /// <param name="entity">Entity to create.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    private async Task ProcessCreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        await this.Context.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update.</typeparam>
    /// <param name="entity">Entity to update.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.ProcessUpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Processes the update of an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update.</typeparam>
    /// <param name="entity">Entity to update.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    private async Task ProcessUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        this.Context.Set<TEntity>().Update(entity);

        var idUserCreatorProperty = this.Context.Entry(entity).Metadata.FindProperty(nameof(IEntity.CreatedBy));
        var createdAtProperty = this.Context.Entry(entity).Metadata.FindProperty(nameof(IEntity.CreatedAt));

        if (idUserCreatorProperty != null)
            this.Context.Entry(entity).Property(nameof(IEntity.CreatedBy)).IsModified = false;

        if (createdAtProperty != null)
            this.Context.Entry(entity).Property(nameof(IEntity.CreatedAt)).IsModified = false;

        await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete.</typeparam>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return this.ProcessDeleteAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Processes the deletion of an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete.</typeparam>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    private async Task ProcessDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        var entity = await this.Context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (entity != null)
        {
            this.Context.Set<TEntity>().Remove(entity);

            await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a set of entities in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create.</typeparam>
    /// <param name="entities">List of entities to create.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public async Task CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        ArgumentNullException.ThrowIfNull(entities);

        await ProcessCreateRangeAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the creation of a set of entities in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create.</typeparam>
    /// <param name="entities">List of entities to create.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    private async Task ProcessCreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        await this.Context.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a set of entities in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update.</typeparam>
    /// <param name="entities">List of entities to update.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public async Task UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        ArgumentNullException.ThrowIfNull(entities);

        this.Context.UpdateRange(entities);

        await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a set of entities in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete.</typeparam>
    /// <param name="entities">List of entities to delete.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public async Task DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase
    {
        ArgumentNullException.ThrowIfNull(entities);

        this.Context.RemoveRange(entities);

        await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Changes the state of a record in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update.</typeparam>
    /// <param name="id">Id of the record to update.</param>
    /// <param name="state">Status that will be assigned to the record if it exists.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public async Task ChangeStateAsync<TEntity>(Guid id, bool state, CancellationToken cancellationToken) where TEntity : class, IEntity
    {
        var entity = await this.Context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (entity != null)
        {
            entity.IsActive = state;

            await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Allows multiple processes in the database in a single transaction.
    /// </summary>
    /// <typeparam name="TResult">Type of data to return if the process is successful.</typeparam>
    /// <param name="process">Process to execute in the transaction flow.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public Task TransactionAsync<TResult>(Func<DbContext, Task> process,  CancellationToken cancellationToken)
    {
        return this.TransactionAsync<TResult>(process, IsolationLevel.ReadUncommitted, cancellationToken);
    }

    /// <summary>
    /// Allows multiple processes in the database in a single transaction.
    /// </summary>
    /// <typeparam name="TResult">Type of data to return if the process is successful.</typeparam>
    /// <param name="process">Process to execute in the transaction flow.</param>
    /// <param name="isolation">Specifies the transaction locking behavior for the connection.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public async Task TransactionAsync<TResult>(Func<DbContext, Task> process, IsolationLevel isolation, CancellationToken cancellationToken)
    {
        var strategy = this.Context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async (cancellation) =>
        {
            using var transaction = await this.Context.Database.BeginTransactionAsync(isolation, cancellation).ConfigureAwait(false);

            await process(this.Context).ConfigureAwait(false);

            await transaction.CommitAsync(cancellation).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }
}