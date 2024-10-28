import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { LayoutService } from './service/app.layout.service';
import { AccountResponseDto } from 'src/proxy/Interfaces/Authentication/account-response-dto';
import { Roles } from 'src/proxy/Enums/roles';
import { Observable } from 'rxjs';
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';
import { ServiceResponse } from 'src/proxy/Interfaces/service-response';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent implements OnInit {

    currentUser: AccountResponseDto = {} as AccountResponseDto;

    response: ServiceResponse<AccountResponseDto> = {} as ServiceResponse<AccountResponseDto>

    currentRole: any;

    menuItems: any[] = []


    model: any[] = [];

    constructor(
        public layoutService: LayoutService,
        public accountService: AccountService,
        public authService: AuthService,
    ) { }

    async ngOnInit(): Promise<void | Observable<void>> {
        const idString: string | null = localStorage.getItem('id');
        const id: number = parseInt(idString as string, 10);


        const accountRequestResponce = await this.accountService.get(id).toPromise()
            .catch(err => {
                try {
                    this.response.data = this.authService.getCurrentUser()
                    return this.response
                } catch (error) {
                    console.log(error)
                    console.log("Error in getting current user")
                    this.authService.logout()
                    return null;

                }
            })

        this.currentUser = accountRequestResponce.data
        this.currentRole = this.currentUser.roleName

        this.menuItems = [
            {
                label: 'Home',
                items: [
                    { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }
                ]
            },
            {
                label: 'User Management',
                items: [
                    {
                        label: 'User Management',
                        icon: 'pi pi-fw pi-users',
                        items: [
                            {
                                label: 'User Accounts',
                                icon: 'pi pi-fw pi-users',
                                routerLink: ['/user-management/accounts'],
                                roles: ['SuperAdmin', 'Admin']
                            }
                        ]
                    },
                ]
            },
        ];


        this.model = processMenuItems(this.menuItems, this.currentRole);

        function processMenuItems(menuItems: any[], currentRole: any): any[] {
            let filteredItems: any[] = [];
            for (let i = 0; i < menuItems.length; i++) {
                const menuItem = menuItems[i];
                if (menuItem.hasOwnProperty('roles')) {
                    let hasRole = false;

                    for (let j = 0; j < menuItem.roles.length; j++) {

                        if (currentRole == menuItem.roles[j]) {

                            hasRole = true;
                            break
                        }
                    }
                    if (hasRole) {
                        // If the item has the required role, add it to the filteredItems array
                        filteredItems.push(menuItem);
                    }
                } else if (menuItem.hasOwnProperty('items')) {
                    // If the item has sub-items, recursively filter them
                    menuItem.items = processMenuItems(menuItem.items, currentRole);
                    if (menuItem.items.length > 0) {
                        // Only add the item if it has remaining sub-items
                        filteredItems.push(menuItem);
                    }
                } else {
                    // If the item doesn't have roles or sub-items, add it directly
                    filteredItems.push(menuItem);
                }
            }
            return filteredItems;
        }



    }
}
