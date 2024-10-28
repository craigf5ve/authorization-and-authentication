import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from 'src/proxy/services/auth.service';
import * as jwt_decode from "jwt-decode";
import { Console } from 'console';
import { environment } from 'src/environments/environment';


@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  url = environment.baseUrl

  constructor(private authService: AuthService) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const modifiedRequest = this.addTokenToRequest(request);

    if (request.url.includes(`${this.url}/Accounts/authenticate`)) {
      return next.handle(request.clone({
        setHeaders: {
          'Content-Type': 'application/json; charset=utf-8',
        },
        withCredentials: true,
      })); // Skip interception
    }

    // If addTokenToRequest returns a HttpRequest<any>, directly handle it with next.handle
    if (!(modifiedRequest instanceof Observable)) {
      return next.handle(modifiedRequest);
    }

    // If addTokenToRequest returns an Observable<HttpRequest<any>>, switchMap to handle it
    return modifiedRequest.pipe(
      switchMap((req: HttpRequest<any>) => next.handle(req))
    );
  }

  private addTokenToRequest(request: HttpRequest<any>): Observable<HttpRequest<any>> {
    const token = this.authService.getJwtToken() || '';
    
    if (!token) {
      const modifiedRequest = request.clone({
        setHeaders: {
          'Content-Type': 'application/json; charset=utf-8',
        },
        withCredentials: true,
      });
      return of(modifiedRequest); // Wrap the modified request in an observable
    }else if(token.length == 0){
    }
  
    const decodedToken: any = jwt_decode.jwtDecode(token);

    if (request.url.includes(`${this.url}/PurchaseOrders/import`)) {
      console.log('Correct Header')
      const modifiedRequest = request.clone({
        setHeaders: {
          'Content-Type': 'multipart/form-data; boundary=something',
          'Accept':'/',
          Authorization: `Bearer ${token}`,
        },
        withCredentials: true,
      });
      return of(modifiedRequest);
    }

    if (decodedToken && typeof decodedToken.exp === 'number' && decodedToken.exp * 1000 <= Date.now()) {
      // Token expired, refresh it
      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          const modifiedRequest = request.clone({
            setHeaders: {
              'Content-Type': 'application/json; charset=utf-8',
                  Authorization: `Bearer ${response.data.jwtToken}`,
              'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,PATCH,OPTIONS',
            },
            withCredentials: true,
          });
          return of(modifiedRequest); // Wrap the modified request in an observable
        })
      );
    } else {
   
      const modifiedRequest = request.clone({
        setHeaders: {
          'Content-Type': 'application/json; charset=utf-8',
              Authorization: `Bearer ${token}`,
          'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,PATCH,OPTIONS',
        },
        withCredentials: true,          
      });
      return of(modifiedRequest); // Wrap the modified request in an observable
    }
  }  

  // private handleRefreshTokenError(
  //   request: HttpRequest<any>,
  //   next: HttpHandler
  // ): Observable<HttpEvent<any>> {
  //   return this.authService.logout().pipe(
  //     switchMap(() => {
  //       console.log('Logged out')
  //       return next.handle(request);
  //     })
  //   );
  // }

  // private handleAuthError(
  //   request: HttpRequest<any>,
  //   next: HttpHandler
  // ): Observable<HttpEvent<any>> {
  //   return this.authService.refreshToken().pipe(
  //     switchMap(() => {
  //       console.log('Token refreshed')
  //       request = this.addTokenToRequest(request);
  //       return next.handle(request);
  //     }),
  //     catchError((error) => {
  //       // Handle refresh token errors here
  //       // You might want to redirect to login page or do other handling
  //       return throwError(error);
  //     })
  //   );
  // }
}
