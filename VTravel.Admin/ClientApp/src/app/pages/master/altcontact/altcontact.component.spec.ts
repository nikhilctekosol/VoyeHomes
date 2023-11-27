import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AltcontactComponent } from './altcontact.component';

describe('AltcontactComponent', () => {
  let component: AltcontactComponent;
  let fixture: ComponentFixture<AltcontactComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AltcontactComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AltcontactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
