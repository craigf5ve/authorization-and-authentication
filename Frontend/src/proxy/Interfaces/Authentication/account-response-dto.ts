import { EntityDto } from "../EntityDtos/entity-dto";

export interface AccountResponseDto extends EntityDto<number>{
    title: string
    firstName: string;
    lastName: string;
    email: string;
    roleName: string;
    departmentId: number;
    departmentName: string;
    locationId: number;
    locationName: string;
    created: Date;
    updated: Date;
    isVerified: boolean;
    isActivated: boolean;
}
