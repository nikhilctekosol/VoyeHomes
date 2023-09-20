import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RateplanComponent } from './rateplan.component';

describe('RateplanComponent', () => {
  let component: RateplanComponent;
  let fixture: ComponentFixture<RateplanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RateplanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RateplanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
