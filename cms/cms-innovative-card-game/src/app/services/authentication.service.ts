import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Token } from '../interfaces/token';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  isLoggedIn: boolean = true;

  private userURL = 'http://localhost:7019/User';

  constructor(private http: HttpClient) {
    if (localStorage.getItem('token')) {
      this.isLoggedIn = true;
    } else {
      this.isLoggedIn = false;
    }
  }

  login(email: string, password: string): Observable<Token> {
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.http.post<Token>(
      this.userURL + `/login?email=${email}&password=${password}`,
      headers
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.isLoggedIn = false;
  }
}
