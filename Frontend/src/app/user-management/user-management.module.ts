import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserManagementRoutingModule } from './user-management-routing.module';
import { AccountsComponent } from './accounts/accounts.component';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { PasswordModule } from 'primeng/password';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { CheckboxModule } from 'primeng/checkbox';
import { TooltipModule } from 'primeng/tooltip';
@NgModule({
  declarations: [
    AccountsComponent,
    UserProfileComponent
  ],
  imports: [
    CommonModule,
    UserManagementRoutingModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    ToastModule,
    DialogModule,
    FormsModule,
    DropdownModule,
    PasswordModule,
    CheckboxModule,
    TooltipModule
  ]
})
export class UserManagementModule { }
