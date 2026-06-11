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
        /// Gets the ShareServiceClient used to interact with Azure File Storage.
        /// </summary>
        public ShareServiceClient Client { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFileFactory"/> class.
        /// </summary>
        /// <param name="options">The file storage options.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="options"/> is null.
        /// </exception>
        public AzureFileFactory(IOptions<FileStorageOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            Options = options.Value;
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
        /// Gets the <see cref="ShareClient"/> for the specified tenant.
        /// </summary>
        /// <param name="tenant">The tenant identifier.</param>
        /// <returns>The <see cref="ShareClient"/> for the specified tenant.</returns>
        public ShareClient GetContainerClient(Guid tenant)
        {
            return this.Client.GetShareClient(tenant.ToString());
        }
    }
}