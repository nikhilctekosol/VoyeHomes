import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NbToastrService, NbComponentStatus, NbDialogContainerComponent, NbDialogService } from '@nebular/theme';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';

@Component({
  selector: 'ngx-sheetapproval',
  templateUrl: './sheetapproval.component.html',
  styleUrls: ['./sheetapproval.component.scss']
})
export class SheetapprovalComponent implements OnInit {

  token: any;
  defaultPropertyId: string = "0";
  selectedPropertyName: string;
  filteredOptions$: Observable<any[]>;
  property = new Property();
  history = new Historydata();
  loadingSave = false;
  properties: any[];
  historylist: any[];
  options: any[];
  @ViewChild('autoInput') input;
  constructor(private http: HttpClient, private router: Router, private authService: NbAuthService
    , private toastrService: NbToastrService, private dialogService: NbDialogService) {
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();


    });

  }

  ngOnInit(): void {
    this.loadProperties();
  }

  private loadProperties() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list-sorted-by-name'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;

            this.options = this.properties;
            this.filteredOptions$ = of(this.options);
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/property/get-list-sorted-by-name', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  onPropSelectionChange($event) {

    this.defaultPropertyId = $event.id;
    this.selectedPropertyName = $event.title.toString().trim();
    // localStorage["default-reservation-property"] = this.defaultPropertyId;
    // localStorage["default-property-name"] = this.selectedPropertyName;
    this.loadProperty();
    this.filteredOptions$ = this.getFilteredOptions(this.selectedPropertyName);
  }

  getFilteredOptions(value: string): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.filter(filterString)),
    );
  }

  private filter(value: string): any[] {
    const filterValue = value.toLowerCase();
    return this.options.filter(optionValue => optionValue.title.toLowerCase().includes(filterValue));
  }

  onChange() {
    this.filteredOptions$ = this.getFilteredOptions(this.input.nativeElement.value);
  }

  loadProperty() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.defaultPropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
          //this.searchdata.property = this.property.id;
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  search() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/get-sheet-history?propid=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.historylist = res.data;
        }

      },
        error => {

          console.log('api/reports/get-sheet-history', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }
  view(id) {
    let h = this.historylist.find(c => c.id = id);

    var win = window.open('', '', 'height=700,width=700');

    win.document.write(h.html);

    win.document.close();

    setTimeout(function () {
      win.print();
      win.close();
    }, 250);
  }
  approve(id) {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/reports/approve-history?id=' + id
      , {}, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;
        alert(res.actionStatus);
        if (res.actionStatus === 'SUCCESS') {
          this.loadingSave = false;
          this.toast('Success', 'Approved successfully!', 'success');
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not approve data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not approve data!', 'danger');

          console.log('api/reports/approve-history', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }

}
class Property {

  id: number;
  propertyTypeId: string;
  destinationId: string;
  title: string;
  thumbnail: string;
  address: string;
  city: string;
  propertyStatus: string;
  sortOrder: number;
  shortDescription: string;
  longDescription: string;
  latitude: number;
  longitude: number;
  state: string;
  country: string;
  displayRadius: number;
  maxOccupancy: number;
  roomCount: number;
  bathroomCount: number;
  metaTitle: string;
  metaKeywords: string;
  metaDescription: string;
  phone: string;
  email: string;
  reserveAlert: string;
  reserveAllowed: string;
}
class Historydata {

  id: number;
  propertyid: string;
  property: string;
  fromDate: string;
  toDate: string;
  html: string;
  isapproved: string;
}
