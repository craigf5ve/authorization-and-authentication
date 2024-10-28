using IronMan.Data.Entities;


namespace IronMan.Data.AggregateRoots
{
    public class FullAuditedAggregateRoot<T> : AuditedAggregateRoot<T>
    {
        public DateTime? LastModificationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public Account? LastModifierUser { get; set; }

        public void PrepareEntityForUpdate (Account account)
        {
            LastModifierUserId = account.Id;
            LastModificationTime = DateTime.UtcNow;
        }

        public void PrepareForCreateAndUpdate (Account account)
        {
            PrepareEntityForCreate(account);
            PrepareEntityForUpdate(account);
        }
    }
}
