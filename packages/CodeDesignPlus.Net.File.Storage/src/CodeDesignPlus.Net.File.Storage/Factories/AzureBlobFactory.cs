using Azure.Identity;
using Azure.Storage.Blobs;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Exceptions;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.File.Storage.Factories;

public class AzureBlobFactory<TKeyUser, TTenant> : IAzureBlobFactory<TKeyUser, TTenant>
{
    public FileStorageOptions Options { get; private set; }
    public IUserContext<TKeyUser, TTenant> UserContext { get; private set; }
    public BlobServiceClient Client { get; private set; }

    public AzureBlobFactory(IOptions<FileStorageOptions> options, IUserContext<TKeyUser, TTenant> userContext)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(userContext, nameof(userContext));
        
        Options = options.Value;
        UserContext = userContext;
    }

    public IAzureBlobFactory<TKeyUser, TTenant> Create()
    {
        if (!this.Options.AzureBlob.Enable)
            throw new FileStorageException("The AzureBlob is not enable");

        if (this.Client != null)
            return this;

        if (this.Options.AzureBlob.UsePasswordLess)
            this.Client = new BlobServiceClient(this.Options.AzureBlob.Uri, new DefaultAzureCredential());
        else
            this.Client = new BlobServiceClient(this.Options.AzureBlob.ConnectionString);

        return this;
    }

    public BlobContainerClient GetContainerClient()
    {
        return this.Client.GetBlobContainerClient(this.UserContext.Tenant.ToString());
    }
}
