import { EntityDto } from "./entity-dto";

export interface AuditedEntityDto<T> extends EntityDto<number> {
    creationTime: Date,
    creatorId: T,
    creatorName: string,
    IsDeleted: boolean,
    DeletionTime: Date,
    DeleterId: T,
    DeleterName: string
}
