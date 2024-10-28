import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ResetPasswordRequestDto } from 'src/proxy/Interfaces/Authentication/reset-password-request-dto';
import { ServiceResponse } from 'src/proxy/Interfaces/service-response';
//import { ResetPasswordRequestDto, ResponseDto } from 'src/proxy/Interfaces/account-dto';
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent {
  resetRequest: ResetPasswordRequestDto = {} as ResetPasswordRequestDto;

  response: ServiceResponse<boolean> = {} as ServiceResponse<boolean>

loading =true
error= false
  resetModal = true;

  constructor(
    private messageService: MessageService,
    private authService: AuthService,
    private route: ActivatedRoute
  ) { }


  ngOnInit(): void {
   this.resetRequest.token  = this.route.snapshot.queryParamMap.get('token') || undefined
  if(!this.resetRequest.token){
    this.resetModal = false;
    this.loading = false;
    this.error=true;
    this.response.message = 'Invalid Operation';
  }

  }

  resetPassword() {
    this.authService.resetPassword(this.resetRequest)
      .subscribe(res => {
        if (res.isSuccess) {
          this.response = res
          this.resetModal=false;
          this.loading = false;
          // this.response.isSuccess = true;
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: res.message,
            life: 3000
          });
        }      
      },
        (error: { message: any; }) => {
          this.error = true;
          this.response.message = 'Invalid Operation';
          this.loading = false;
          
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error.message,
            life: 3000
          });
        });
  }
}
