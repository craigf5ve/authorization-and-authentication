import { Roles } from "src/proxy/Enums/roles";

export interface RegisterRequestDto {
    title: string;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
    role: Roles;
    department: number;
    acceptTerms: boolean;
}
