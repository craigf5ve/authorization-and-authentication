import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VerifyEmailRequestDto } from 'src/proxy/Interfaces/Authentication/verify-email-request-dto';
import { ServiceResponse } from 'src/proxy/Interfaces/service-response';
import { AuthService } from 'src/proxy/services/auth.service';

@Component({
  selector: 'app-activate-account',
  templateUrl: './activate-account.component.html',
  styleUrls: ['./activate-account.component.scss']
})
export class ActivateAccountComponent {
  verifyEmailRequest: VerifyEmailRequestDto = {} as VerifyEmailRequestDto

  response: ServiceResponse<boolean> = {} as ServiceResponse<boolean>;
  loading = false;
  error = false;

  constructor(
    private authService: AuthService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {

    this.verifyEmailRequest.token = this.route.snapshot.queryParamMap.get('token') ?? '';
    if (this.verifyEmailRequest.token) {
      this.loading = true;
      this.authService.activateAccount(this.verifyEmailRequest)
        .subscribe((res: ServiceResponse<boolean>) => {
            if (res.isSuccess) {
              this.response = res;
              this.loading = false;
              // this.response.isSuccess = true;
            }
            // else{
            //   this.response = res;
            //   this.loading = false;
            //   // this.response.isSuccess = false;
            // }
          },
          (error: any) => {
            this.response.isSuccess = false;
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
}
