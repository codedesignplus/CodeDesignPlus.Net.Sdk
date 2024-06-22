namespace CodeDesignPlus.Net.Exceptions.Models;

public record ErrorResponse(string TraceId, Layer Layer)
{
    public List<ErrorDetail> Errors { get; } = [];

    public void AddError(string code, string message, string field)
    {
        this.Errors.Add(new ErrorDetail(code, field, message));
    }
}
