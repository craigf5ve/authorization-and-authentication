import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Roles } from 'src/proxy/Enums/roles';
import { CreateRequestDto } from 'src/proxy/Interfaces/Authentication/create-request-dto';
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  account: CreateRequestDto = {} as CreateRequestDto;

  roles: any;

  loading = false;

  passwordMatch = true;

  passwordStatus: string = '';

  isValid: any

  validationMessage: string = '';

  constructor(
    private accountService: AccountService,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router
  ) { }

  ngOnInit(): void {

    this.roles = Object.values(Roles);

  }

  routeToLogin() {
    this.router.navigate(['/auth/login']);

  }

  private passwordValidation(account: any): boolean {
    // Check if password matches confirm password
    if (account.password !== account.confirmPassword) {
      this.validationMessage = 'Passwords do not match';
      this.passwordStatus = 'ng-invalid ng-dirty'
      return false;
    }

    // Check minimum length (7 characters)
    if (account.password.length < 6) {
      this.validationMessage = 'Password must be at least 6 characters long';
      this.passwordStatus = 'ng-invalid ng-dirty'

      return false;
    }

    // Regex pattern: at least one lowercase letter, one uppercase letter, one special character, one digit, and no spaces
    let passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[\w\S]{6,}$/;

    // Check if password matches regex pattern
    if (!passwordRegex.test(account.password)) {
      this.validationMessage = "Password must contain at least:\n one lowercase letter, one uppercase letter,one special characters and one digit, with no spaces";
      this.passwordStatus = 'ng-invalid ng-dirty'

      return false;
    }

    // All checks passed, password is valid
    this.validationMessage = '';
    this.passwordStatus = 'ng-valid'
    return true;
  }

  register() {

    const isValid = this.passwordValidation(this.account);

    if (isValid) {
      // Call your registration API or service method here
      try {
        this.saveRegistration();
        // Registration successful, you can redirect or show a success message
      } catch (error) {
        // Handle registration error, such as displaying an error message
        console.error('Registration error:', error);
      }
    }
  }

  saveRegistration() {
    this.loading = true;
    this.authService.register(this.account)
      .subscribe((res: { isSuccess: any; message: any; }) => {
        if (res.isSuccess) {
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/auth/login']);
          }, 4000); // Delay for 2000 milliseconds (2 seconds)
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: res.message,
            life: 3000
          });
        } else {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Registration Failed',
            detail: 'Unknown error occurred',
            life: 3000
          });
        }

      },
        (error: { message: any; }) => {
          this.loading = false;

          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error.message || 'An unexpected error occurred',
            life: 3000
          });
        }
      );

  }
}
