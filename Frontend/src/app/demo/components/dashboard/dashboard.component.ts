import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountResponseDto } from 'src/proxy/Interfaces/Authentication/account-response-dto';
import { AccountService } from 'src/proxy/services/account.service';
import { MessageService } from 'primeng/api';
import { LayoutService } from 'src/app/layout/service/app.layout.service';

@Component({
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit, OnDestroy {

  currentUser: AccountResponseDto = {} as AccountResponseDto;

  refreshLocationModal = false;

  refreshDepartmentModal = false;

  closeBatchPricesModal = false;

  fromDate!: Date;

  startingDate!: Date;

  departmentId!: number;

  constructor(
    private accountService: AccountService,
    private messageService: MessageService,
    public layoutService: LayoutService
  ) { }

  async ngOnInit() {
    const idString: string | null = localStorage.getItem('id');
    const id: number = parseInt(idString as string, 10);

    const res = await this.accountService.get(id).toPromise();
    this.currentUser = res.data;
  }

  ngOnDestroy() {
  }

  hideDialog() {
    this.refreshLocationModal = false;
    this.refreshDepartmentModal = false;
    this.closeBatchPricesModal = false;
  }
}
