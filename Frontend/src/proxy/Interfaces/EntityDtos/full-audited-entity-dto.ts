import { AuditedEntityDto } from "./audited-entity-dto";

export interface FullAuditedEntityDto<T> extends AuditedEntityDto<number>{
    lastModificationTime: Date,
    lastModifierUserId: T,
    lastModifierUserName: string,
}
