import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../interfaces/user';
import { UserTeam } from '../interfaces/user-team';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private userURL = 'http://localhost:7019/';

  constructor(private http: HttpClient) {}

  createUser(user: User) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<any>(this.userURL + 'User', user, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }

  getUsers(teamID: string) {
    let token: string | null = localStorage.getItem('token');

    return this.http.get<User[]>(this.userURL + 'User/GetUsers?TeamId=' + teamID, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteUserFromTeam(userTeam: UserTeam) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<string>(this.userURL + 'User/DeleteUserFromTeam', userTeam, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
