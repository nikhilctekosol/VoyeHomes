import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild, ViewChild, Renderer2 } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import {
  NbDialogService, NbToastrService, NbComponentStatus, NbDialogContainerComponent
} from '@nebular/theme';
import { CalendarOptions, Calendar, DateSelectArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'ngx-inventorynew',
  templateUrl: './inventorynew.component.html',
  styleUrls: ['./inventorynew.component.scss']
})
export class InventorynewComponent implements OnInit {

  calendarPlugins = [dayGridPlugin];
  token: any;
  properties: any[];
  loadingSave = false;
  defaultPropertyId: string = "0";
  selectedrate: number = 0;
  property = new Property();
  rooms: any[];
  room = new Room();
  isLoadingRooms = false;
  currentYear: string ;

  fromdate: string = "";
  todate: string = "";

  calendarOptionsArray: CalendarOptions[] = [];
  @ViewChild('rateplanDialogNew') rateplanDialogNew: TemplateRef<any>;
  dialogRef: any;
  activerateplans: any[];
  assignedplans: any[];
  rpData = new RPData();
  filteredOptions$: Observable<any[]>;
  selectedPropertyName: string ;

  @ViewChild('autoInput') input;

  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router, private renderer: Renderer2,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();
    });
    // Create array of month strings like ['2023-01', '2023-02', ...]

  }

  ngOnInit(): void {
    this.loadProperties();
    // if (localStorage["default-inventory-property"]) {
    //  this.defaultPropertyId = localStorage["default-inventory-property"];
    //  this.selectedPropertyName = localStorage["default-property-name"];
    // }

    this.currentYear = new Date().getFullYear().toString();
    this.loadcalendar();

    this.loadProperty();
  }


  loadcalendar() {
    this.calendarOptionsArray.length = 0;

    for (let i = 1; i <= 12; i++) {


      const calendarOptions: CalendarOptions = {
        initialView: 'dayGridMonth',
        views: {
          dayGridMonth: {
            titleFormat: { month: 'short' },
          }
        },

        headerToolbar: {
          left: '',
          center: 'title',
          right: ''
        },
        initialDate: `${this.currentYear}-${String(i).padStart(2, '0')}-01`,
        fixedWeekCount: false,
        selectable: true ,
        select: this.handleDateSelect.bind(this),
        validRange: this.calculateValidRange(
          new Date(parseInt(this.currentYear), i - 1 , 1))
      };
      this.calendarOptionsArray.push(calendarOptions);

    }

  }

  calculateValidRange(month: Date) {
    const startOfMonth = new Date(month.getFullYear(), month.getMonth(), 1);
    const endOfMonth = new Date(month.getFullYear(), month.getMonth() + 1, 1);

    return {
      start: startOfMonth,
      end: endOfMonth
    };
  }

  yearchange(count) {
    this.currentYear = parseInt(this.currentYear) + count;

    this.loadcalendar();
    this.loadactiverateplans();
    this.getassignedrateplans();
  }

  clear() {
    if (this.fromdate != "") {
      var from = this.formatDate(this.fromdate);
      const dayElement = document.querySelector(`.fc-day[data-date="${from}"]`);
      if (dayElement) {
        dayElement.classList.remove('clicked-day');
      }
    }
    if (this.todate != "") {
      var to = this.formatDate(this.todate);
      const dayElement = document.querySelector(`.fc-day[data-date="${to}"]`);
      if (dayElement) {
        dayElement.classList.remove('clicked-day');
      }
    }

    this.fromdate= "";
    this.todate= "";
  }

  handleDateSelect(selectInfo: DateSelectArg) {

    var check = this.formatDate(selectInfo.startStr);
    var today = this.formatDate(new Date());


    if (check >= today) {

      if (this.fromdate == "") {
        this.fromdate = check;
        const dayElement = document.querySelector(`.fc-day[data-date="${this.fromdate}"]`);
        if (dayElement) {
          dayElement.classList.add('clicked-day');
        }
      }
      else {
        var from = this.formatDate(this.fromdate);
        if (from <= check) {
          this.todate = check;
          const dayElement = document.querySelector(`.fc-day[data-date="${this.todate}"]`);
          dayElement.classList.add('clicked-day');

          this.dialogRef = this.dialogService.open(
            this.rateplanDialogNew,
            { context: { title: 'Select Rate plan' } });

        }
        else {
          this.toast('Error', 'The day before from date is not selectable!', 'danger');
        }
      }


    }
    else {

      this.toast('Error', 'Previous day is not selectable!', 'danger');
    }


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

  datestring(date) {
    const parts = date.split('-');
    if (parts.length === 3) {
      return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    return date; // Return the original date if the format is not as expected
  }

  private loadProperties() {
    this.loadingSave = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;
            this.filteredOptions$ = of(this.properties);
          }
        }

      },
        error => {
          this.loadingSave = false;
          console.log('api/property/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  onPropertySelected(event) {
    localStorage["default-inventory-property"] = this.defaultPropertyId;

    this.loadProperty();
  }

  loadactiverateplans() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {
          this.activerateplans = res.data;
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadProperty() {

    this.removeClassFromDayElements('clicked-day');

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.defaultPropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
          this.rooms = [];
          this.room = new Room();
          this.loadRooms();
          // this.loadInventories();
          this.loadactiverateplans();
          this.getassignedrateplans();
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

  onInventorySubmit() {

    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.rpData.fromDate = this.formatDate((new Date(this.fromdate)).toString());
    this.rpData.toDate = this.formatDate((new Date(this.todate)).toString());
    this.rpData.propertyId = this.property.id.toString();
    this.rpData.rateplan = this.selectedrate;


    this.http.post('api/rateplan/assign-rateplan'
      , this.rpData, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {


          this.loadcalendar();
          this.fromdate = "";
          this.todate = "";
          this.selectedrate = 0;

          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
          this.loadcalendar();
          this.getassignedrateplans();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;

          console.log('api/inventory/create-bulk', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  getassignedrateplans() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-assignedplan-list?id=' + this.property.id
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {
          this.assignedplans = res.data;

          for (let i = 0; i < this.assignedplans.length; i++) {
            const dayElements = document.querySelectorAll(`.fc-day[data-date="${this.assignedplans[i].invDate}"]`);
            dayElements.forEach((dayElement) => {
              this.renderer.setStyle(dayElement, 'background-color', this.assignedplans[i].rp_color);
              this.renderer.setStyle(dayElement, 'color', this.getContrastingTextColor(this.assignedplans[i].rp_color));
              this.renderer.setAttribute(dayElement, 'title', this.assignedplans[i].rp_name);


            });
          }

        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  getContrastingTextColor(backgroundColor: string ): string {
    // Convert the background color to RGB
    const rgb = parseInt(backgroundColor.slice(1), 16);
    const r = (rgb >> 16) & 0xff;
    const g = (rgb >> 8) & 0xff;
    const b = (rgb >> 0) & 0xff;

    // Calculate the relative luminance of the background color
    const luminance = 0.299 * r + 0.587 * g + 0.114 * b;

    // Determine the text color based on the luminance
    return luminance > 128 ? 'black' : 'white';
  }

  removeClassFromDayElements(className: string ) {

    // Find all day elements with the specified class and remove it
    const dayElements = document.querySelectorAll(`.fc-day.${className}`);
    dayElements.forEach((dayElement) => {
      this.renderer.removeClass(dayElement, className);
    });
  }

  onSelectionChange($event) {

    // console.log(JSON.stringify($event));
    this.defaultPropertyId = $event.id;
    this.selectedPropertyName = $event.title.toString().trim();
    localStorage["default-inventory-property"] = this.defaultPropertyId;
    localStorage["default-property-name"] = this.selectedPropertyName;
    this.loadProperty();
    this.filteredOptions$ = this.getFilteredOptions(this.selectedPropertyName);
  }

  private filter(value: string ): any[] {
    const filterValue = value.toLowerCase();
    return this.properties.filter(optionValue => optionValue.title.toLowerCase().includes(filterValue));
  }

  getFilteredOptions(value: string ): Observable<any[]> {
    return of(value).pipe(
      map(filterString => this.filter(filterString)),
    );
  }

  onChange() {

    // this.input.nativeElement.setAttribute("area-expanded", "true");
    this.filteredOptions$ = this.getFilteredOptions(this.input.nativeElement.value);
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }

}

class Property {

  id: string ;
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
class RPData {

  id: number;
  fromDate: string ;
  toDate: string ;
  propertyId: string ;
  mode: string ;
  rateplan: number;

}
