import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SheetapprovalComponent } from './sheetapproval.component';

describe('SheetapprovalComponent', () => {
  let component: SheetapprovalComponent;
  let fixture: ComponentFixture<SheetapprovalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SheetapprovalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SheetapprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
