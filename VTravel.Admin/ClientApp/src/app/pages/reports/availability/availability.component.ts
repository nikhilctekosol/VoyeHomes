import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NbToastrService, NbComponentStatus, NbDialogContainerComponent } from '@nebular/theme';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';

@Component({
  selector: 'availability',
  templateUrl: './availability.component.html',
  styleUrls: ['./availability.component.scss']
})
export class AvailabilityComponent implements OnInit {

  token: any;
  defaultPropertyId: string = "0";
  defaultRoomId: string = "0";
  selectedPropertyName: string ;
  selectedRoomName: string ;
  filteredOptions$: Observable<any[]>;
  filteredRooms$: Observable<any[]>;
  options: any[];
  roomoptions: any[];
  property = new Property();
  rooms: any[];
  room = new Room();
  isLoadingRooms = false;
  properties: any[];
  loadingSave = false;
  searchdata = new Searchdata();
  tabledata: string ;
  href: string ;


  @ViewChild('autoInput') input;
  @ViewChild('autoInput1') input1;

  constructor(private http: HttpClient, private router: Router, private authService: NbAuthService
    , private toastrService: NbToastrService) {
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      this.searchdata.property = 0;
      this.searchdata.room = 0;

    });

  }

  ngOnInit(): void {

    this.loadProperties();
    this.searchdata.fromDate = new Date();
  }

  onPropSelectionChange($event) {

    this.defaultPropertyId = $event.id;
    this.selectedPropertyName = $event.title.toString().trim();
    // localStorage["default-reservation-property"] = this.defaultPropertyId;
    // localStorage["default-property-name"] = this.selectedPropertyName;
    this.loadProperty();
    this.filteredOptions$ = this.getFilteredOptions(this.selectedPropertyName);
  }
  onRoomSelectionChange($event) {
    this.defaultRoomId = $event.id;
    this.selectedRoomName = $event.title.toString().trim();
    localStorage["default-reservation-room"] = this.defaultRoomId;
    localStorage["default-room-name"] = this.selectedRoomName;
    this.loadRoom();
    this.filteredRooms$ = this.getroomFilteredOptions(this.selectedRoomName);
  }


  getFilteredOptions(value: string ): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.filter(filterString)),
    );
  }
  getroomFilteredOptions(value: string ): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.roomfilter(filterString)),
    );
  }
  private filter(value: string ): any[] {
    const filterValue = value.toLowerCase();
    return this.options.filter(optionValue => optionValue.title.toLowerCase().includes(filterValue));
  }
  private roomfilter(value: string ): any[] {
    const filterValue = value.toLowerCase();
    return this.roomoptions.filter(optionValue => optionValue.title.toLowerCase().includes(filterValue));
  }

  onChange() {
    this.filteredOptions$ = this.getFilteredOptions(this.input.nativeElement.value);
  }
  onRoomChange() {
    this.filteredRooms$ = this.getroomFilteredOptions(this.input1.nativeElement.value);
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

  loadProperty() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.defaultPropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
          this.searchdata.property = this.property.id;
          this.rooms = [];
          this.room = new Room();
          this.loadRooms();
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  loadRooms() {
    this.isLoadingRooms = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.get('api/property/get-room-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {


          if (res.data.length > 0) {
            this.rooms = res.data;

            this.roomoptions = this.rooms;
            this.filteredRooms$ = of(this.roomoptions);

          }
        }
        this.isLoadingRooms = false;
      },
        error => {

          this.isLoadingRooms = false;
          console.log('api/property/get-room-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  loadRoom() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-room?propid=' + this.property.id + '&id=' + this.defaultRoomId
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {

          this.room = res.data[0];

          this.searchdata.room = this.room.id;
        }
      },
        error => {

          console.log('api/property/get-room', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  search() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reports/availability?propid=' + this.searchdata.property
      + '&room=' + this.searchdata.room + '&fromdate=' + this.searchdata.fromDate.getFullYear().toString()
      + '-' + (this.searchdata.fromDate.getMonth() + 1).toString() + '-'
      + this.searchdata.fromDate.getDate().toString()
      , { headers: headers }).subscribe((res: any) => {
        if (res.actionStatus == 'SUCCESS') {

          this.tabledata = res.data;
          this.href = 'api/property/excelavailability?propid=' + this.searchdata.property
            + '&room=' + this.searchdata.room + '&fromdate=' + this.searchdata.fromDate.getFullYear().toString()
            + '-' + (this.searchdata.fromDate.getMonth() + 1).toString() + '-'
            + this.searchdata.fromDate.getDate().toString();

        }
      },
        error => {

          console.log('api/property/get-room', error)
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
      this.tabledata
      '</table>' +
      '</body> ' +
      '</html>';
    window.location.href = location + window.btoa(excelTemplate);
  }


  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}

class Property {

  id: number ;
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

class Room {

  id: number;
  roomTypeId: string ;
  propertyId: number;
  title: string ;
  description: string ;
  typeName: string ;
}
class Searchdata {

  property: number | undefined;
  room: number | undefined;
  fromDate: Date | undefined;
}
class Report {

  property: string ;
  room: string ;
  d1: string ;
  d2: string ;
  d3: string ;
  d4: string ;
  d5: string ;
  d6: string ;
  d7: string ;
}
