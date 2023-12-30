import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { AdminCmsComponent } from './components/admin-cms/admin-cms.component';
import { TeamLeaderCmsComponent } from './components/team-leader-cms/team-leader-cms.component';

import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NavigationComponent } from './components/navigation/navigation.component';
import { GameCmsComponent } from './components/game-cms/game-cms.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {MatDialogModule} from '@angular/material/dialog'; 
import {MatButtonModule} from '@angular/material/button';
import { ErrorComponent } from './components/modals/error/error.component';
import { SuccessComponent } from './components/modals/success/success.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    AdminCmsComponent,
    TeamLeaderCmsComponent,
    NavigationComponent,
    GameCmsComponent,
    ErrorComponent,
    SuccessComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatDialogModule, 
    MatButtonModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
