namespace CodeDesignPlus.Net.Mongo.Diagnostics.Abstractions
{
    public interface IActivityService
    {
        ActivitySource ActivitySource { get; }

        Activity StartActivity(string name, ActivityKind kind);
        bool AddActivity(int id, Activity activity);
        bool TryGetActivity(int id, out Activity activity);
        bool RemoveActivity(int id, out Activity activity);
    }
}