import { EntityDto } from "../EntityDtos/entity-dto";

export interface AuthenticateResponseDto extends EntityDto<number>{
    title: string
    firstName: string;
    lastName: string;
    email: string;
    roleName: string;
    departmentName: string;
    created: Date;
    updated: Date;
    isVerified: boolean;
    isActivated: boolean;
    jwtToken: string;
    refreshToken: string;
}
