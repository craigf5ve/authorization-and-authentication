import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { AuthenticateRequestDto } from 'src/proxy/Interfaces/Authentication/authenticate-request-dto';
import { ForgotPasswordRequestDto } from 'src/proxy/Interfaces/Authentication/forgot-password-request-dto';
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  account: AuthenticateRequestDto = {} as AuthenticateRequestDto

  forgotRequest: ForgotPasswordRequestDto = {} as ForgotPasswordRequestDto

  forgotModal = false;

  loading = false;

  constructor(
    private accountService: AccountService,
    private messageService: MessageService,
    private authService: AuthService,
    private router: Router
  ) { }

  authenticate(account: any) {
    this.loading = true;
    this.authService.authenticate(account)
      .subscribe((res: { isSuccess: any; data: { jwtToken: string; id: string; }; message: string | undefined; }) => {
          if (res.isSuccess) {
            this.router.navigate([localStorage.getItem('returnUrl') || '']);
            this.loading = false;
            localStorage.removeItem('returnUrl');

            localStorage.setItem('jwtToken', res.data.jwtToken);
            localStorage.setItem('id', res.data.id);

            localStorage.setItem('currentUser', JSON.stringify(res.data));

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
              summary: 'Login Failed',
              detail: res ? res.message : 'Unknown error occurred',
              life: 3000
            });
          }
        },
        (error: { message: any; }) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Incorrent Email or Password',
            life: 3000
          });
        });

  }


  forgotPassword() {
    this.authService.forgotPassword(this.forgotRequest)
      .subscribe(res => {
        this.forgotModal = false
        if (res) {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: res.message,
            life: 3000
          });
        }
        else {
          this.messageService.add({
            severity: 'error',
            summary: 'Login Failed',
            detail: res.message,
            life: 3000
          });
        }
      },
        (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error.message,
            life: 3000
          });

        })
  }


}
