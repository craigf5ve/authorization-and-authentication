import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { throwError, BehaviorSubject, Observable } from 'rxjs';
import { catchError, switchMap, filter, take, finalize, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import * as jwt_decode from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  url = environment.baseUrl


  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService

  ) { }

  // ...

  public refreshingToken: boolean = false;
  public refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  //#region Helper Methods  

  async decodeToken(jwtToken: any): Promise<boolean> {
    return new Promise<boolean>((resolve, reject) => {
      try {
        // Decode the token
        const decodedToken: any = jwt_decode.jwtDecode(jwtToken)

        // Check if 'exp' property is defined
        if (decodedToken && typeof decodedToken.exp === 'number') {
          // Compare expiry to current time
          if (decodedToken.exp * 1000 <= Date.now()) {
            resolve(false); // Token has expired
          } else {
            resolve(true); // Token is still valid
          }
        } else {
          // 'exp' property is undefined or not a number
          console.error('Token expiration not found or invalid');
          resolve(false); // Consider token as expired to be safe
        }
      } catch (error) {
        console.error('Error decoding token:', error);
        resolve(false); // Consider token as expired if decoding fails
      }
    });
  }

  getRefreshToken(): string | null {
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
      const cookie = cookies[i].trim();
      if (cookie.startsWith('refreshToken=')) {
        return cookie.substring('refreshToken='.length);
      }
    }
    return null;
  }

  getJwtToken(): string | null {
    return localStorage.getItem('jwtToken')
  }

  public handleError(error: any, request: Observable<any>): Observable<any> {
      console.error('An error occurred:', error);
      return throwError(error);    
  }

  public getCurrentUser(){
    return JSON.parse(localStorage.getItem('currentUser') || '{}')
  }
  
  // clearRefreshToken() {
  //   //Clear It from cookies
  //   document.cookie = `refreshToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  // }

  clearJwtToken() {
    //Clear It from cookies
    localStorage.removeItem('jwtToken')
  }

  clearLocalStorage() {
    //Clear LocalStorage
    // localStorage.clear()
    localStorage.removeItem('jwtToken');
          localStorage.removeItem('id');
          localStorage.removeItem('currentUser');
  }

  public logout(){
    this.clearLocalStorage()
    this.router.navigate(['/login'])
  }

  //#endregion


  //#region API Requests

  register(itemDto: any): any {
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/register`, body);
  }

  forgotPassword(itemDto: any) {
    const body = JSON.stringify(itemDto);

    return this.http.post<any>(`${this.url}/Accounts/forgot-password`, body);
  }

  resetPassword(itemDto: any) {
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/reset-password`, body);
  }

  verifyEmail(itemDto: any) {
    const body = JSON.stringify(itemDto);

    return this.http.post<any>(`${this.url}/Accounts/verify-email`, body);
  }

  // ...
  // AllowAnonymous
  authenticate(itemDto: any): any {
    let returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/';
    localStorage.setItem('returnUrl', returnUrl);
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/authenticate`, body);
  }

  // ...
  // AllowAnonymous
  validateToken(itemDto: any) {
    const body = JSON.stringify(itemDto);
    // this.decodeToken(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/validate-token`, body);
  }

  // AllowAnonymous
  refreshToken(): Observable<any> {
    this.clearLocalStorage()
    return this.http.post<any>(`${this.url}/Accounts/refresh-token`, {}).pipe(
      catchError(this.handleError),
      tap((response: { isSuccess: any; data: { jwtToken: string; id: string; }; }) => {
        if (response.isSuccess) {
          localStorage.setItem('jwtToken', response.data.jwtToken);
          localStorage.setItem('id', response.data.id);
          localStorage.setItem('currentUser', JSON.stringify(response.data));
        }
      })
    );
  }

  revokeToken(itemDto: any) {
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/revoke-token`, body);
  }


  //AllowAnonymous
  activateAccount(itemDto: any): any {
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/activate-account`, body);
  }

  activateAccountByAdmin(itemDto: any): any {
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts/activate-account-by-admin`, body);
  }
  
  //#endregion


}