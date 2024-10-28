import { Component } from '@angular/core';
import { MessageService } from 'primeng/api';
import { AccountResponseDto } from 'src/proxy/Interfaces/Authentication/account-response-dto';
import { ResetPasswordRequestDto } from 'src/proxy/Interfaces/Authentication/reset-password-request-dto';
import { UpdateRequestDto } from 'src/proxy/Interfaces/Authentication/update-request-dto';
import { AccountService } from 'src/proxy/services/account.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent {

  currentUser: AccountResponseDto = {} as AccountResponseDto;

  updateUser: UpdateRequestDto = {} as UpdateRequestDto;

  resetRequest: ResetPasswordRequestDto = {} as ResetPasswordRequestDto;

  resetModal = false;

  updateModal = false;

  constructor(
    private accountService: AccountService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {

    const idString: string | null = localStorage.getItem('id');
    const id: number = parseInt(idString as string, 10);

    this.accountService.get(id)
      .subscribe(res => {
        if (res.isSuccess) {
          this.currentUser = res.data
        }
        else {
          this.messageService.add({
            severity: 'error',
            summary: 'Profile GET Failed',
            detail: res.message,
            life: 3000
          });
        }
      });

  }


  edit(item : any) {
    this.updateUser = item
    this.updateUser.role = item.roleName
    this.updateModal = true;
  }

  update() {
    this.accountService.update(this.updateUser)
      .subscribe(res => {
        if (res.isSuccess) {
          this.currentUser = res.data 
          this.messageService.add({
            severity: 'success',
            summary: 'Profile Updated',
            detail: res.message,
            life: 3000
          });
        }
        else {
          this.messageService.add({
            severity: 'error',
            summary: 'Profile Update Failed',
            detail: res.message,
            life: 3000
          });
        }
      });

  }
}
