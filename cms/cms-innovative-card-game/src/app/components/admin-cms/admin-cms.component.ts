import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Organization } from 'src/app/interfaces/organization';
import { Team } from 'src/app/interfaces/team';
import { User } from 'src/app/interfaces/user';
import { OrganizationService } from 'src/app/services/organization.service';
import { TeamService } from 'src/app/services/team.service';
import { UserService } from 'src/app/services/user.service';
import { ErrorComponent } from '../modals/error/error.component';
import { SuccessComponent } from '../modals/success/success.component';

@Component({
  selector: 'app-admin-cms',
  templateUrl: './admin-cms.component.html',
  styleUrls: ['./admin-cms.component.scss']
})
export class AdminCmsComponent {
    protected organizationName: string = "";
    protected teamName: string = "";
    protected firstname: string;
    protected lastname: string;
    protected email: string;
    protected password: string;

    constructor(private organizationService: OrganizationService, private teamService: TeamService, private userService: UserService, private dialog: MatDialog) { 
    }

    createOrganization() {
      let organization: Organization = {
        name: this.organizationName,
        organizationID: 0
      };

      let organizationID: string;
      
      this.organizationService.createOrganization(organization).subscribe({
          next: r => organizationID = r,
          error: err => {
            this.dialog.open(ErrorComponent)
            console.log(err)
          },
          complete: () => {
              this.dialog.open(SuccessComponent)
          }
      });
    }
}
