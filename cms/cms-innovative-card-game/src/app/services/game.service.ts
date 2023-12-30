import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Deck } from '../interfaces/deck';
import { Card } from '../interfaces/card';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  private apiURLDeck = 'http://localhost:7238/';
  private apiURLCard = 'http://localhost:7143/';

  constructor(private http: HttpClient) {}

  createDeck(deck: Deck) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<any>(
      this.apiURLDeck + 'CreateDeck',
      deck,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      }
    );
  }

  getDecks(teamID: string) {
    let token: string | null = localStorage.getItem('token');

    return this.http.get<Deck[]>(this.apiURLDeck + 'GetAllDecks?TeamID=' + teamID, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }

  createCard(card: Card) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<any>(
      this.apiURLCard + 'CreateCardInDeck',
      card,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      }
    );
  }

  getAllCardsFromDeck(deckID: string) {
    let token: string | null = localStorage.getItem('token');

    return this.http.get<Card[]>(this.apiURLCard + 'GetAllCardsFromDeck?DeckID=' + deckID, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteDeck(deck: Deck) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<any>(this.apiURLDeck + 'DeleteDeck', deck, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteCard(card: Card) {
    let token: string | null = localStorage.getItem('token');

    return this.http.post<any>(this.apiURLCard + 'DeleteCardFromDeck', card, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
