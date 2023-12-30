import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Organization } from 'src/app/interfaces/organization';
import { Team } from 'src/app/interfaces/team';
import { GameService } from 'src/app/services/game.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { TeamService } from 'src/app/services/team.service';
import { ErrorComponent } from '../modals/error/error.component';
import { SuccessComponent } from '../modals/success/success.component';
import { Deck } from 'src/app/interfaces/deck';
import { Card } from 'src/app/interfaces/card';

@Component({
  selector: 'app-game-cms',
  templateUrl: './game-cms.component.html',
  styleUrls: ['./game-cms.component.scss']
})
export class GameCmsComponent {

  constructor(private organizationService: OrganizationService, private gameService: GameService, private teamService: TeamService, private dialog: MatDialog) {}
  
  organizations: Organization[] = [];
  deckName: string = "";
  teams: Team[] = [];
  organizationID: string = "";
  teamID: string = "";
  decks: Deck[] = [];
  deckID: string = "";
  frontSideText: string = "";
  backSideText: string = "";
  cards: Card[] = [];

    ngOnInit(): void {
      this.organizationService.getOrganizations().subscribe((organizations) => {
        this.organizations = organizations;
        console.log(this.organizations);
      });
  }

  updateOrganizationID(event: any) {
    this.organizationID = event.target.value;
    this.teamService.getTeams(this.organizationID).subscribe((teams) => {
      this.teams = teams;
    });
  }

  getDecks() {
    this.gameService.getDecks(this.teamID).subscribe((decks) => {
      this.decks = decks;
      console.log(this.decks);
    });
  }

  getAllCardsFromDeck() {
    this.gameService.getAllCardsFromDeck(this.deckID).subscribe((cards) => {
      this.cards = cards;
      console.log(this.cards);
    });

    console.log(this.deckID);
  }

  createDeck() {
    let deck: Deck = {
      name: this.deckName,
      teamID: this.teamID
    }

    console.log(deck);
    
    this.gameService.createDeck(deck).subscribe({
      next: r => console.log(r),
      error: err => {console.log(err); this.dialog.open(ErrorComponent)},
      complete: () => this.dialog.open(SuccessComponent)
    });
    
  }

  createCard() {
    let card: Card = {
      frontSideText: this.frontSideText,
      backSideText: this.backSideText,
      deckID: this.deckID
    }

    console.log(card);

    this.gameService.createCard(card).subscribe({
      next: r => console.log(r),
      error: err => {console.log(err); this.dialog.open(ErrorComponent)},
      complete: () => this.dialog.open(SuccessComponent)
    });
  }

  deleteDeck() {
    let deck: Deck = {
      deckID: this.deckID,
      teamID: this.teamID
    }

    this.gameService.deleteDeck(deck).subscribe({
      next: r => console.log(r),
      error: err => {console.log(err); this.dialog.open(ErrorComponent)},
      complete: () => this.dialog.open(SuccessComponent)
    });
  }

  deleteCard(cardID: string) {
    let card: Card = {
      cardID: cardID,
      deckID: this.deckID
    }

    this.gameService.deleteCard(card).subscribe({
      next: r => console.log(r),
      error: err => {console.log(err); this.dialog.open(ErrorComponent)},
      complete: () => this.dialog.open(SuccessComponent)
    });
  }

}
