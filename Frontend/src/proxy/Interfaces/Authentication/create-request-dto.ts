export interface CreateRequestDto {
    title: string
    firstName: string
    lastName: string
    role: string
    department: number
    location: number
    email: string
    password: string
    confirmPassword: string
    acceptTerms: boolean
}
