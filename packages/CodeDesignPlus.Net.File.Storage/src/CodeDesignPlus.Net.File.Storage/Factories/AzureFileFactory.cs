namespace CodeDesignPlus.Net.File.Storage.Factories
{
    /// <summary>
    /// Factory class for creating and managing Azure File Storage clients.
    /// </summary>
    public class AzureFileFactory : IAzureFileFactory
    {
        /// <summary>
        /// Gets the file storage options.
        /// </summary>
        public FileStorageOptions Options { get; private set; }

        /// <summary>
        /// Gets the user context.
        /// </summary>
        public IUserContext UserContext { get; private set; }

        /// <summary>
        /// Gets the ShareServiceClient used to interact with Azure File Storage.
        /// </summary>
        public ShareServiceClient Client { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFileFactory"/> class.
        /// </summary>
        /// <param name="options">The file storage options.</param>
        /// <param name="userContext">The user context.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="options"/> or <paramref name="userContext"/> is null.
        /// </exception>
        public AzureFileFactory(IOptions<FileStorageOptions> options, IUserContext userContext)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(userContext);

            Options = options.Value;
            UserContext = userContext;
        }

        /// <summary>
        /// Creates and initializes the Azure File Storage client.
        /// </summary>
        /// <returns>The current instance of <see cref="AzureFileFactory"/>.</returns>
        /// <exception cref="FileStorageException">
        /// Thrown when Azure File Storage is not enabled in the options.
        /// </exception>
        public IAzureFileFactory Create()
        {
            if (!this.Options.AzureFile.Enable)
                return this;

            if (this.Client != null)
                return this;

            if (this.Options.AzureFile.UsePasswordLess)
                this.Client = new ShareServiceClient(this.Options.AzureFile.Uri, new DefaultAzureCredential());
            else
                this.Client = new ShareServiceClient(this.Options.AzureFile.ConnectionString);

            return this;
        }

        /// <summary>
        /// Gets the <see cref="ShareClient"/> for the current tenant.
        /// </summary>
        /// <returns>The <see cref="ShareClient"/> for the current tenant.</returns>
        public ShareClient GetContainerClient()
        {
            return this.Client.GetShareClient(this.UserContext.Tenant.ToString());
        }
    }
}