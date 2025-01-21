namespace CodeDesignPlus.Net.Core.Test.Helpers.Context
{
    public class FakeEntity : IEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid Tenant { get; set; }
        public Instant CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Instant? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
