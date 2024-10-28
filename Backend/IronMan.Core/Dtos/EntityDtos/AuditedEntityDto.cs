namespace IronMan.Core.Dtos.EntityDtos
{
    public class AuditedEntityDto<T> : EntityDto<T>
    {
        public DateTime? CreationTime { get; set; }
        public int? CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int? DeleterId { get; set; }
        public string? DeleterName { get; set; }
    }
}
