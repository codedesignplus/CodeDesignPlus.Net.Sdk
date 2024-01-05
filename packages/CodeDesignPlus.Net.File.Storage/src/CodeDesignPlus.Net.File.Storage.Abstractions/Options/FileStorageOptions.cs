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
    /// <summary>
    /// Gets or sets the Azure Blob options
    /// </summary>
    public AzureBlobOptions AzureBlob { get; set; }
    /// <summary>
    /// Gets or sets the Azure File options
    /// </summary>
    public AzureFileOptions AzureFile { get; set; }
    /// <summary>
    /// Gets or sets the Local options
    /// </summary>
    public LocalOptions Local { get; set; }
    /// <summary>
    /// Gets or sets the Uri to download the file
    /// </summary>
    [Required]
    public Uri UriDownload { get; set; }

    /// <summary>
    /// Validate the options
    /// </summary>
    /// <param name="validationContext">Context of the validation</param>
    /// <returns>A collection that holds failed-validation information.</returns>
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
