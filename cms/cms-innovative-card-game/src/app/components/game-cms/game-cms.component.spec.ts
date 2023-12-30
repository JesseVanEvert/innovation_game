import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameCmsComponent } from './game-cms.component';

describe('GameCmsComponent', () => {
  let component: GameCmsComponent;
  let fixture: ComponentFixture<GameCmsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GameCmsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GameCmsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
