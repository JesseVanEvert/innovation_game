import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Organization } from '../interfaces/organization';

@Injectable({
  providedIn: 'root',
})
export class OrganizationService {
  private organizationURL = 'http://localhost:7000/';

  constructor(private http: HttpClient) {}

  createOrganization(organization: Organization) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<string>(
      this.organizationURL + 'CreateOrganization',
      organization,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      }
    );
  }

  getOrganizations() {
    let token: string | null = localStorage.getItem('token');

    return this.http.get<Organization[]>(this.organizationURL + 'GetOrganizations', {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
