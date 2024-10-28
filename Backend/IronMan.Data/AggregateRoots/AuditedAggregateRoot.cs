using IronMan.Data.Entities;


namespace IronMan.Data.AggregateRoots
{
    public class AuditedAggregateRoot<T> : BasicAggregateRoot<T>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int? CreatorId { get; set; }
        public Account? Creator { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletionTime { get; set; }
        public int? DeleterId { get; set; }
        public Account? Deleter { get; set; }

        public void PrepareEntityForDelete (Account account)
        {
            IsDeleted = true;
            DeleterId = account.Id;
            DeletionTime = DateTime.UtcNow;
        }

        public void PrepareEntityForCreate (Account account)
        {
            CreatorId = account.Id;
            CreationTime = DateTime.UtcNow;
        }
    }
}
