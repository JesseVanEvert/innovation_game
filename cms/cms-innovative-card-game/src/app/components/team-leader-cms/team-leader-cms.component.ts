import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Organization } from 'src/app/interfaces/organization';
import { Team } from 'src/app/interfaces/team';
import { OrganizationService } from 'src/app/services/organization.service';
import { TeamService } from 'src/app/services/team.service';
import { ErrorComponent } from '../modals/error/error.component';
import { SuccessComponent } from '../modals/success/success.component';
import { UserService } from 'src/app/services/user.service';
import { User } from 'src/app/interfaces/user';
import { UserTeam } from 'src/app/interfaces/user-team';

@Component({
  selector: 'app-team-leader-cms',
  templateUrl: './team-leader-cms.component.html',
  styleUrls: ['./team-leader-cms.component.scss']
})
export class TeamLeaderCmsComponent {
  constructor(private organizationService: OrganizationService, private teamService: TeamService, private userService: UserService, private dialog: MatDialog) {}
  organizations: Organization[] = [];
  teams: Team[] = [];
  users: User[] = [];
  organizationID: string = "";
  teamID: string = "";
  userId: string = "";
  teamName: string = "";
  firstname: string = "";
  lastname: string = "";
  email: string = "";
  password: string = "";

  ngOnInit(): void {
     this.organizationService.getOrganizations().subscribe((organizations) => {
       this.organizations = organizations;
       console.log(this.organizations);
     });
  }

  deleteUserFromTeam() {
    let userTeam: UserTeam = {
      teamId: this.teamID,
      userId: this.userId,
    }

    console.log(userTeam);

    this.userService.deleteUserFromTeam(userTeam).subscribe({
      next: r => console.log(r),
      error: err => {console.log(err); this.dialog.open(ErrorComponent)},
      complete: () => this.dialog.open(SuccessComponent)
    });
  }

  getUsers() {
    this.userService.getUsers(this.teamID).subscribe((users) => {
      this.users = users;
      console.log(this.users);
    });
  }

  createUser() {
    let user: User = {
      teamID: this.teamID,
      firstname: this.firstname,
      lastname: this.lastname,
      email: this.email,
      password: this.password
    };

    console.log(user);

    this.userService.createUser(user).subscribe({
      next: r => console.log(r),
      error: err => {console.log(err); this.dialog.open(ErrorComponent)},
      complete: () => this.dialog.open(SuccessComponent)
    });
  }

  createTeam() {

    let team: Team = {
      name: this.teamName,
      organizationID: this.organizationID
    };

    console.log(team);

    this.teamService.createTeam(team).subscribe({
      next: r => console.log(r),
      error: err => {
        console.log(err)
        this.dialog.open(ErrorComponent)
      },
      complete: () => this.dialog.open(SuccessComponent)
    });
  }

  updateOrganizationID(event: any) {
    this.organizationID = event.target.value;
    //Get teams by organizationID
    this.teamService.getTeams(this.organizationID).subscribe((teams) => {
      this.teams = teams;
    });
  }

  updateTeamID(event: any) {
    this.teamID = event.target.value;
    console.log(this.teamID);
  }
}
