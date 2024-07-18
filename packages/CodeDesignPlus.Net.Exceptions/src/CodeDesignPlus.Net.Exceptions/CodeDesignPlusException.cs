namespace CodeDesignPlus.Net.Exceptions;

public class CodeDesignPlusException : Exception
{
    public Layer Layer { get; set; }
    public string Code { get; set; }

    public CodeDesignPlusException(Layer layer, string code)
    {
        this.Code = code;
        this.Layer = layer;
    }

    public CodeDesignPlusException(Layer layer, string code, string message) : base(message)
    {
        this.Code = code;
        this.Layer = layer;
    }

    public CodeDesignPlusException(Layer layer, string code, string message, Exception innerException) : base(message, innerException)
    {
        this.Code = code;
        this.Layer = layer;
    }
}
