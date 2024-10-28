import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { throwError } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})

export class AccountService {
  url = environment.baseUrl

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private authService: AuthService
  ) { }

  create(itemDto: any): Observable<any> {
    const body = JSON.stringify(itemDto);
    return this.http.post<any>(`${this.url}/Accounts`, body);
  }

  get(id: number) {
    return this.http.get<any>(`${this.url}/Accounts/${id}`);
  }
  
  getAll(): Observable<any> {
    return this.http.get<any>(`${this.url}/Accounts`);
  }
  
  update(itemDto: any): Observable<any> {
    const body = JSON.stringify(itemDto);
    return this.http.put<any>(`${this.url}/Accounts/${itemDto.id}`, body);
  }
  
  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.url}/Accounts/${id}`);
  }
}
