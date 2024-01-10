using Azure.Identity;
using Azure.Storage.Files.Shares;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Exceptions;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.File.Storage.Factories;

public class AzureFileFactory<TKeyUser, TTenant> : IAzureFlieFactory<TKeyUser, TTenant>
{
    public FileStorageOptions Options { get; private set; }
    public IUserContext<TKeyUser, TTenant> UserContext { get; private set; }
    public ShareServiceClient Client { get; private set; }

    public AzureFileFactory(IOptions<FileStorageOptions> options, IUserContext<TKeyUser, TTenant> userContext)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(userContext, nameof(userContext));

        Options = options.Value;
        UserContext = userContext;
    }

    public IAzureFlieFactory<TKeyUser, TTenant> Create()
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

