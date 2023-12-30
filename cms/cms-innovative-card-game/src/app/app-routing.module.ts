import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { AdminCmsComponent } from './components/admin-cms/admin-cms.component';
import { TeamLeaderCmsComponent } from './components/team-leader-cms/team-leader-cms.component';
import { AdminGuard } from './admin.guard';
import { GameCmsComponent } from './components/game-cms/game-cms.component';

const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: 'admin-cms',
    component: AdminCmsComponent,
    //canActivate: [AdminGuard],
  },
  {
    path: 'team-leader-cms',
    component: TeamLeaderCmsComponent,
    //canActivate: [AdminGuard],
  },
  {
    path: 'game-cms',
    component: GameCmsComponent,
    //canActivate: [AdminGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
