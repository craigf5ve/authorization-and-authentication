import { Component, OnInit } from '@angular/core';
import { error } from 'console';
import { VerifyEmailRequestDto } from 'src/proxy/Interfaces/Authentication/verify-email-request-dto';
import { ServiceResponse } from 'src/proxy/Interfaces/service-response';
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';

@Component({
  selector: 'app-verify-email',
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.scss']
})
export class VerifyEmailComponent {

  verifyEmailRequest: VerifyEmailRequestDto = {} as VerifyEmailRequestDto

  response: ServiceResponse<boolean> = {} as ServiceResponse<boolean>
  // token: string | null = null;

  loading = true
  error = false

  constructor(
    private accountsService: AccountService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.verifyEmailRequest.token = this.getTokenFromURL() || null;
    if (this.verifyEmailRequest.token) {
      this.loading = true;
      this.authService.verifyEmail(this.verifyEmailRequest)
        .subscribe((res) => {
          this.response = res;
          this.loading = false;
          // this.response.isSuccess = true;
        },
          (error: any) => {
            // this.response.isSuccess = false;
            this.response.message = 'Verification not Successful';
            this.loading = false;
          }
        );
    } else {
      this.error = true;
      this.response.message = 'Invalid Operation';
      this.loading = false;
      console.log(this.loading);
    }
  }

  private getTokenFromURL(): any {
    // Get the current URL
    const url = window.location.href;

    // Use URLSearchParams to extract the token parameter
    const params = new URLSearchParams(url.split('?')[1]);
    const token = params.get('token');

    return token;
  }

}
