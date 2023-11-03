import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReservationnewComponent } from './reservationnew.component';

describe('ReservationnewComponent', () => {
  let component: ReservationnewComponent;
  let fixture: ComponentFixture<ReservationnewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReservationnewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReservationnewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
