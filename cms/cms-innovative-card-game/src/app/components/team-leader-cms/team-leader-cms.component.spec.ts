import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamLeaderCmsComponent } from './team-leader-cms.component';

describe('TeamLeaderCmsComponent', () => {
  let component: TeamLeaderCmsComponent;
  let fixture: ComponentFixture<TeamLeaderCmsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeamLeaderCmsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TeamLeaderCmsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
