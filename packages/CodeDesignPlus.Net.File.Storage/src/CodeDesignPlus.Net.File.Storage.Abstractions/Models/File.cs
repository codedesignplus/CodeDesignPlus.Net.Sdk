namespace CodeDesignPlus.Net.File.Storage.Abstractions.Models;

public class File
{
    public string Extension { get => System.IO.Path.GetExtension(this.FullName); }
    public string FullName { get; set; }
    public string Name { get => System.IO.Path.GetFileNameWithoutExtension(this.FullName); }
    public Path Path { get; set; }
    public long Size { get; set; }
    public string Version { get; set; }
    public bool Renowned { get; set; }
    public bool Overwrite { get; set; }
    public ApacheMime Mime { get => ApacheMime.ApacheMimes.FirstOrDefault(x => x.Extension.Contains(this.Extension)); }

    public File(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            throw new ArgumentNullException(nameof(fullName));

        this.FullName = fullName;
    }

    public Dictionary<string, string> GetMetadata(Uri uri)
    {
        return new Dictionary<string, string>
        {
            { "FullName", this.FullName },
            { "Name", this.Name },
            { "Extension", this.Extension },
            { "Size", this.Size.ToString() },
            { "Version", this.Version },
            { "Renowned", this.Renowned.ToString() },
            { "Overwrite", this.Overwrite.ToString() },
            { "Mime", this.Mime.ToString() },
            { "Uri", uri.ToString()}
        };
    }

    public Dictionary<string, string> GetTags<TTenant>(TTenant tenant)
    {
        return new Dictionary<string, string>
        {
            { "Name", this.Name },
            { "Mime", this.Mime.ToString() },
            { "Tenant", tenant.ToString() }
        };
    }
}
