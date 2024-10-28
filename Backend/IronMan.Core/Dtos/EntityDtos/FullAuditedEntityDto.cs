namespace IronMan.Core.Dtos.EntityDtos
{
    public class FullAuditedEntityDto<T> : AuditedEntityDto<T>
    {
        public DateTime? LastModificationTime { get; set; }
        public int? LastModifierUserId { get; set; }
        public string? LastModifierUserName { get; set; }
    }
}
