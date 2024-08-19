namespace CodeDesignPlus.Net.File.Storage.Factories;

public class AzureFileFactory : IAzureFileFactory
{
    public FileStorageOptions Options { get; private set; }
    public IUserContext UserContext { get; private set; }
    public ShareServiceClient Client { get; private set; }

    public AzureFileFactory(IOptions<FileStorageOptions> options, IUserContext userContext)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(userContext);

        Options = options.Value;
        UserContext = userContext;
    }

    public IAzureFileFactory Create()
    {
        if (!this.Options.AzureFile.Enable)
            throw new FileStorageException("The AzureBlob is not enable");

        if (this.Client != null)
            return this;

        if (this.Options.AzureFile.UsePasswordLess)
            this.Client = new ShareServiceClient(this.Options.AzureFile.Uri, new DefaultAzureCredential());
        else
            this.Client = new ShareServiceClient(this.Options.AzureFile.ConnectionString);

        return this;
    }

    public ShareClient GetContainerClient()
    {
        return this.Client.GetShareClient(this.UserContext.Tenant.ToString());
    }
}

