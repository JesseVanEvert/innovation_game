import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Team } from '../interfaces/team';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private teamURL = 'http://localhost:7281/';

  constructor(private http: HttpClient) {}

  createTeam(team: Team) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<string>(this.teamURL + 'CreateTeam', team, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }

  getTeams(organizationID: string) {
    let token: string | null = localStorage.getItem('token');

    return this.http.get<Team[]>(this.teamURL + 'GetTeams?OrganizationId=' + organizationID, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
