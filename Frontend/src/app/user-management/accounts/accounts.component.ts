import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { Roles } from 'src/proxy/Enums/roles';
import { AccountResponseDto } from 'src/proxy/Interfaces/Authentication/account-response-dto';
import { ActivateAccountByAdminRequestDto } from 'src/proxy/Interfaces/Authentication/activate-account-by-admin-request-dto';
import { CreateRequestDto } from 'src/proxy/Interfaces/Authentication/create-request-dto';
import { UpdateRequestDto } from 'src/proxy/Interfaces/Authentication/update-request-dto';
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.scss']
})
export class AccountsComponent {
  accounts: AccountResponseDto[] = [];

  account: CreateRequestDto = {} as CreateRequestDto;

  selectedAccount: UpdateRequestDto = {} as UpdateRequestDto

  deleteAccount: AccountResponseDto = {} as AccountResponseDto

  accountActivation: ActivateAccountByAdminRequestDto = {} as ActivateAccountByAdminRequestDto;

  locationIcon: string = 'pi pi-angle-down';

  departmentIcon: string = 'pi pi-angle-down';

  createModal = false

  updateModal = false;

  deleteModal = false;

  cols: any;

  roles: any;

  selectedAccountRole: any;

  constructor(
    private accountService: AccountService,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router
  ) { }

  ngOnInit(): void {
   
    this.roles = Object.values(Roles);

    this.accountService.getAll()
      .subscribe(res => {
        if (res.isSuccess) {
          this.accounts = res.data
        }
        else {
          this.messageService.add({
            severity: 'error',
            summary: 'Getting Failed',
            detail: res.message,
            life: 3000
          });
        }
      });

  }

  hideDialog() {

  }

  delete(account: AccountResponseDto) {
    this.deleteAccount = { ...account }
    this.deleteModal = true
  }


  edit(item: any) {
    this.updateModal = true
    this.selectedAccount = item;
    this.selectedAccount.role = item.roleName
  }

  activateAccount(item: any) {
    this.accountActivation.id = item.id
    this.authService.activateAccountByAdmin(this.accountActivation)
      .subscribe((res: { isSuccess: any; message: any; }) => {
        if (res.isSuccess) {
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
            summary: 'Activation Failed',
            detail: res.message,
            life: 3000
          });
        }
      }),
      (error: { message: any; }) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: error.message,
          life: 3000
        });
      }
  }


  register() {
    this.accountService.create(this.account)
      .subscribe(
        (res: { data: AccountResponseDto; isSuccess: any; message: string | undefined; }) => {
          if (res.isSuccess) {
            this.router.navigate(['/auth/login']);
            this.createModal = false;
            this.accounts = [...this.accounts, res.data]
            this.account = {} as CreateRequestDto;
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: res.message,
              life: 3000
            });
          } else {
            this.messageService.add({
              severity: 'error',
              summary: 'Registration Failed',
              detail: res ? res.message : 'Unknown error occurred',
              life: 3000
            });
          }
        },
        (error: { message: any; }) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error.message || 'An unexpected error occurred',
            life: 3000
          });
        }
      );
  }

  update() {
    this.accountService.update(this.selectedAccount)
      .subscribe(res => {
        if (res.isSuccess) {
          this.updateModal = false

          var index = this.accounts.findIndex(x => x.email === this.selectedAccount.email);
          this.accounts.splice(index, 1);
          this.accounts.splice(index, 0, res.data);

          this.selectedAccount = {} as UpdateRequestDto;
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
            summary: 'Update Failed',
            detail: res.message,
            life: 3000
          });
        }
      });
  }

  confirmDelete() {
    this.accountService.delete(this.deleteAccount.id)
      .subscribe(res => {
        if (res.isSuccess) {
          this.deleteModal = false;
          //removing the deleted item from the list
          var index = this.accounts.findIndex(x => x.id === res.id);
          this.accounts.splice(index, 1);
          this.deleteAccount = {} as AccountResponseDto;
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
      })
  }

  create() {
    this.createModal = true
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }
}
