using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

/// <summary>
/// Model that will obtain the request data to paginate the results.
/// </summary>
public class Parameters
{
    /// <summary>
    /// Current page.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "The {0} field not is valid.")]
    public int CurrentPage { get; set; }
    /// <summary>
    /// Number of records on the page.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "The {0} field not is valid.")]
    public int PageSize { get; set; }
    /// <summary>
    /// Maximum number of pages.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "The {0} field not is valid.")]
    public int MaxPage { get; set; }
}