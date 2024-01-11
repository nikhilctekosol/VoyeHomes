import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NbToastrService, NbComponentStatus, NbDialogContainerComponent, NbDialogService } from '@nebular/theme';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';

@Component({
  selector: 'ngx-settlement',
  templateUrl: './settlement.component.html',
  styleUrls: ['./settlement.component.scss']
})
export class SettlementComponent implements OnInit {

  token: any;
  defaultPropertyId: string = "0";
  selectedPropertyName: string ;
  filteredOptions$: Observable<any[]>;
  property = new Property();
  options: any[];
  properties: any[];
  searchdata = new Searchdata();
  historydata = new Historydata();
  settlementdata: any[] = [];
  reservationdata: any[] = [];
  loadingSave = false;
  dialogRef: any;
  monthlist: any;
  id: string;
  isapproved: string;
  html: string;
  @ViewChild('autoInput') input;
  constructor(private http: HttpClient, private router: Router, private authService: NbAuthService
    , private toastrService: NbToastrService, private dialogService: NbDialogService) {
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      this.searchdata.property = 0;

    });

  }

  ngOnInit(): void {
    this.loadProperties();
    this.loadMonths();
    var date = new Date();
    this.searchdata.fromDate = new Date(date.getFullYear(), date.getMonth(), 1);
    this.searchdata.toDate = new Date(date.getFullYear(), date.getMonth() + 1, 0);
  }

  private loadProperties() {

    this.loadingSave = true ;
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

  loadMonths() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/getmonths'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.monthlist = res.data;

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

  loaddate() {
    this.searchdata.fromDate = new Date(parseInt(this.searchdata.month.split(' ')[1]), parseInt(this.searchdata.month.split(' ')[0]) - 1, 1);
    this.searchdata.toDate = new Date(parseInt(this.searchdata.month.split(' ')[1]), parseInt(this.searchdata.month.split(' ')[0]), 0);

  }

  onPropSelectionChange($event) {

    this.defaultPropertyId = $event.id;
    this.selectedPropertyName = $event.title.toString().trim();
    // localStorage["default-reservation-property"] = this.defaultPropertyId;
    // localStorage["default-property-name"] = this.selectedPropertyName;
    this.loadProperty();
    this.filteredOptions$ = this.getFilteredOptions(this.selectedPropertyName);
  }

  getFilteredOptions(value: string ): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.filter(filterString)),
    );
  }

  private filter(value: string ): any[] {
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
          this.searchdata.property = this.property.id;
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
    this.http.get('api/reports/settlement?propid=' + this.searchdata.property
      + '&fromdate=' + this.formatDate(this.searchdata.fromDate)
      + '&todate=' + this.formatDate(this.searchdata.toDate)
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {

          this.settlementdata = res.data;

          this.http.get('api/reports/get-sheet-status?id=' + this.searchdata.property
            + '&fromdate=' + this.formatDate(this.searchdata.fromDate)
            + '&todate=' + this.formatDate(this.searchdata.toDate)
            , { headers: headers }).subscribe((res: any) => {

              if (res.actionStatus == 'SUCCESS') {

                this.id = res.data.split('&&')[0];
                this.isapproved = res.data.split('&&')[1] == '' ? 'N' : res.data.split('&&')[1];
                this.html = res.data.split('&&')[2];
              }
            });
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not fetch data!', 'danger');

        }
      },
        error => {

          console.log('api/reports/settlement', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  exportToExcel() {
    var location = 'data:application/vnd.ms-excel;base64,';
    var excelTemplate = '<html> ' +
      '<head> ' +
      '<meta http-equiv="content-type" content="text/plain; charset=UTF-8"/> ' +
      '</head> ' +
      '<body> ' +
      '<table>' +
      document.getElementById("tblData").innerHTML
    '</table>' +
      '</body> ' +
      '</html>';
    window.location.href = location + window.btoa(excelTemplate);
  }

  formatDate(date) {
    var d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
  }

  viewpopup(tagDialogNew: TemplateRef<any>, resid) {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/reservationdata?resid=' + resid
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {

          this.reservationdata = res.data;

          this.dialogRef = this.dialogService.open(
            tagDialogNew,
            { context: { title: 'Reservation Details - ' + resid } });
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not fetch data!', 'danger');

        }
      },
        error => {

          console.log('api/reports/reservationdata', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });


  }

  getsheet() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/sheet?propid=' + this.searchdata.property
      + '&fromdate=' + this.formatDate(this.searchdata.fromDate)
      + '&todate=' + this.formatDate(this.searchdata.toDate)
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {
          if (res.data != '') {
            this.loadingSave = false;
            var win = window.open('', '', 'height=700,width=700');

            win.document.write(res.data);

            win.document.close();

            setTimeout(function () {
              win.print();
              win.close();
            }, 250);
          }
          else {

            this.loadingSave = false;
            this.toast('Error', 'No data found!', 'danger');
          }

        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not fetch data!', 'danger');

        }
      },
        error => {

          console.log('api/reports/settlement', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  view() {

    var win = window.open('', '', 'height=700,width=700');

    win.document.write(this.html);

    win.document.close();

    setTimeout(function () {
      win.print();
      win.close();
    }, 250);
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}
class Property {

  id: number;
  propertyTypeId: string ;
  destinationId: string ;
  title: string ;
  thumbnail: string ;
  address: string ;
  city: string ;
  propertyStatus: string ;
  sortOrder: number;
  shortDescription: string ;
  longDescription: string ;
  latitude: number;
  longitude: number;
  state: string ;
  country: string ;
  displayRadius: number;
  maxOccupancy: number;
  roomCount: number;
  bathroomCount: number;
  metaTitle: string ;
  metaKeywords: string ;
  metaDescription: string ;
  phone: string ;
  email: string ;
  reserveAlert: string ;
  reserveAllowed: string ;
}
class Searchdata {

  property: number | undefined;
  fromDate: Date | undefined;
  toDate: Date | undefined;
  month: string;
}
class Historydata {

  propertyid: string;
  fromDate: string;
  toDate: string;
  //html: string;
}
