using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options;


/// <summary>
/// Options to setting of the FileStorage
/// </summary>
public class FileStorageOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "FileStorage";

    public AzureBlobOptions AzureBlob { get; set; }
    public AzureFileOptions AzureFile { get; set; }
    public LocalOptions Local { get; set; }

    public Uri UriDownload { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validations = new List<ValidationResult>();

        if (this.AzureBlob != null)
            validations.AddRange(this.AzureBlob.Validate(validationContext));

        if (this.AzureFile != null)
            validations.AddRange(this.AzureFile.Validate(validationContext));

        if (this.Local != null)
            validations.AddRange(this.Local.Validate(validationContext));

        return validations;
    }
}
