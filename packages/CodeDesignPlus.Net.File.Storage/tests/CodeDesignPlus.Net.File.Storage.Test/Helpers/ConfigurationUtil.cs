using CodeDesignPlus.Net.File.Storage.Abstractions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Helpers;

public static class OptionsUtil
{
    public static readonly FileStorageOptions FileStorageOptions = new()
    {
        Enable = true,
        Name = nameof(Abstractions.Options.FileStorageOptions.Name),
        Email = $"{nameof(Abstractions.Options.FileStorageOptions.Name)}@codedesignplus.com"
    };

}
