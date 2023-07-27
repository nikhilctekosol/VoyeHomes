import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbToastrService, NbComponentStatus, NbDialogContainerComponent  } from '@nebular/theme';
import { CalendarOptions, FullCalendarComponent, EventClickArg, EventApi, DateSelectArg } from '@fullcalendar/angular';


@Component({
  selector: 'reservation',
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.scss'],
})


export class ReservationComponent implements OnInit {

  displayMode = 'INVENTORY';
  isLoadingInventories = false;
  inventories: any[];
  channels: any[]; 
  deleteDialogRef: any;
  reserveDialogRef: any;
  dialogRef: any;
  reservations: any[];
  rooms: any[];
  room = new Room();
  reservData = new ReservData();
  defaultPropertyId: string = "0";
  isLoading = false;
  loadingSave = false;
  loadingDelete = false;
  isLoadingRooms = false;
  isLoadingReservations = false;
  properties: any[];
  currentEvents: EventApi[] = [];
  token: any;
  user: any;
  property = new Property();
  @ViewChild('calendar') calendarComponent: FullCalendarComponent;
  @ViewChild('reserveDialogNew') reserveDialogNew: TemplateRef<any>;
  @ViewChild('reserveDialog') reserveDialog: TemplateRef<any>;

  calendarOptions: CalendarOptions = {
    displayEventTime: false,
    height: '100%',
    contentHeight: 'auto',
    headerToolbar: {
      left: 'today prev,next',
      center: 'title',
      right: ''
    },
    
    views: {
      dayGridMonth: { // name of view
        titleFormat: { year: 'numeric', month: 'short' },
       
        // other view-specific options here
      }
    },
    initialDate: new Date(),
    //validRange: {
    //  start: new Date(),
    //},
    navLinks: false, // can click day/week names to navigate views
    selectable: true,
    selectMirror: true,
    select: this.handleDateSelect.bind(this),
    eventClick: this.handleEventClick.bind(this),
    eventsSet: this.handleEvents.bind(this),
    editable: true,
    dayMaxEvents: true, // allow "more" link when too many events
    
    events: [
    ]
  };

  handleDateSelect(selectInfo: DateSelectArg) {

    var check = this.formatDate(selectInfo.startStr);
    var today = this.formatDate(new Date());

  
    if (check >= today) {

      //check if there is a reservation already
      var maxAvailableQty = this.checkIfInventory(selectInfo.startStr, selectInfo.endStr);
      if (maxAvailableQty>0) {
        //const cust_name = prompt('Please enter customer name');
        const calendarApi = selectInfo.view.calendar;

        calendarApi.unselect(); // clear date selection

        if (this.user.role == 'ADMIN') {
          this.reservData = new ReservData();
          this.reservData.maxAvailableQty = maxAvailableQty;
          this.reservData.fromDate = selectInfo.startStr;
          this.reservData.toDate = selectInfo.endStr;
          this.reservData.roomId = this.room.id.toString();
          this.reservData.propertyId = this.property.id.toString();

          this.dialogRef = this.dialogService.open(
            this.reserveDialogNew,
            { context: { title: 'Create Reservation' } });
        }

       

        //this.reservData.custName = cust_name;
        //this.reservData.custEmail = "nasar@gmail.com";
        //this.reservData.custPhone = "12345";


       // if (cust_name) {

         

         // this.createReservation();

          //calendarApi.addEvent({
          //  title,
          //  start: selectInfo.startStr,
          //  end: selectInfo.endStr,
          //  allDay: selectInfo.allDay
          //});
       // }
      }

     
    }

 
  }

  handleEventClick(clickInfo: EventClickArg) {

    //console.log(JSON.stringify(clickInfo));

    this.reservData = this.reservations.find(_obj => _obj.id == clickInfo.event.id);

    var maxAvailableQty = this.checkIfInventory(this.reservData.fromDate, this.reservData.toDate);
   // console.log(this.reservData.noOfRooms);
    this.reservData.maxAvailableQty =  parseInt(this.reservData.noOfRooms) + maxAvailableQty;

    this.reservData.fromDate = this.formatDate(this.reservData.fromDate);
    this.reservData.toDate = this.formatDate(this.reservData.toDate);

    this.dialogRef = this.dialogService.open(
      this.reserveDialog,
      { context: { title: 'Update Reservation' } });

  }

  handleEvents(events: EventApi[]) {
    this.currentEvents = events;
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

  checkIfReservation(startStr, endStr) {

    const startDate = this.formatDate(startStr);
    const endDate = this.formatDate(endStr);

    for (let i = 0; i < this.reservations.length; i++) {
      const resFromDate = this.formatDate(this.reservations[i].fromDate);
      const resToDate = this.formatDate(this.reservations[i].toDate);

      if (((startDate >= resFromDate) && (startDate < resToDate))
        || ((endDate > resFromDate) && (endDate <= resToDate)))
      {
        return true;
      }
      else if (((resFromDate >= startDate) && (resFromDate < endDate ))
        || ((resToDate > startDate) && (resToDate <= endDate ))) {
        return true;
      }
    }
   
    return false;
  }

  checkIfInventory(startStr, endStr) {

    const startDate = this.formatDate(startStr);
    const endDate = this.formatDate(endStr);

    var date1Tmp = new Date(startDate);
    var date2Tmp = new Date(endDate);

   

    var minQty = this.getAvailableQty(date1Tmp);
    date1Tmp.setDate(date1Tmp.getDate() + 1);
    while (date1Tmp < date2Tmp) {
      
      var minQtyTmp = this.getAvailableQty(date1Tmp);
      if (minQtyTmp < minQty) {
        minQty = minQtyTmp;
      }
      date1Tmp.setDate(date1Tmp.getDate() + 1);
     } 

    

    return minQty;
  }

  getAvailableQty(dateTmp) {
    for (let i = 0; i < this.inventories.length; i++) {

      var invDate = new Date(this.inventories[i].invDate);
      //console.log(dateTmp);
      //console.log(invDate);
      if (this.formatDate(dateTmp) == this.formatDate(invDate)) {
       
        return this.inventories[i].totalQty - this.inventories[i].bookedQty;
      }
    }

    return 0;
  }

  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {


    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();


    });

    

    this.authService.onTokenChange()
      .subscribe((token: NbAuthJWTToken) => {

        if (token.isValid()) {
          this.user = token.getPayload();

         

        }

      });

  }

  ngOnInit() {

    if (localStorage["default-reservation-property"]) {
      this.defaultPropertyId= localStorage["default-reservation-property"];
    }

   

    this.loadProperty();


    setTimeout(function () {
      window.dispatchEvent(new Event('resize'))
    }, 1);

  }

  selectRoom(item) {
  
    this.room = item;
    this.calendarOptions.events = [];
    this.loadInventories();
  }

  showBookings(evt) {
    var target = evt.target;
    if (target.checked) {
      this.displayMode = 'BOOKINGS';
      this.loadReservations();
    } else {
      
    }
  }

  showInventory(evt) {
    var target = evt.target;
    if (target.checked) {
      this.displayMode = 'INVENTORY';
      this.loadInventories();
    } else {

    }
  }
  loadReservations() {

    this.reservations = [];
    this.displayReservations();
    this.isLoadingReservations = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/reservation/get-list?roomId='+this.room.id
      , { headers: headers }).subscribe((res: any) => {

        

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.reservations = res.data;

          }
          if (this.displayMode == 'INVENTORY') {
            this.displayInventories();
          }
          else if (this.displayMode == 'BOOKINGS') {
            this.displayReservations();
          }
        }
        this.isLoadingReservations = false;
      },
        error => {

          this.isLoadingReservations = false;
          console.log('api/reservation/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  displayReservations() {
    var events = [];
    for (let i = 0; i < this.reservations.length; i++) {
      var color = "#3788d8";
      if (this.reservations[i].isHostBooking == 'Y') {
        color = "#800080";
      } else {
        if (this.reservations[i].bookingChannelId == 45) {
          color = "#008000";
        }
      }
      events.push(
        {
          id: this.reservations[i].id,
          title: this.reservations[i].custName,
          start: this.reservations[i].fromDate,
          end: this.reservations[i].toDate,
          color: color,
          allDay: true
        });
    }
    this.calendarOptions.events = events;
  }

  loadInventories() {

   
    this.isLoadingInventories = true;

    this.inventories = [];

    
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/inventory/get-list-room?roomId=' + this.room.id 
      , { headers: headers }).subscribe((res: any) => {

        //console.log(JSON.stringify(res));

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.inventories = res.data;
            this.loadReservations();
          }

          
        }
        this.isLoadingInventories = false;
      },
        error => {

          this.isLoadingInventories = false;
          console.log('api/inventory/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  displayInventories() {
    var events = [];
    for (let i = 0; i < this.inventories.length; i++) {

      

      events.push(
        {
          id: this.inventories[i].id,
          title:  this.inventories[i].totalQty - this.inventories[i].bookedQty+" Rooms",
          start: this.inventories[i].invDate,
          end: this.inventories[i].invDate,         
          allDay: false,
          editable: false
        });


   
     
    }
    this.calendarOptions.events = events;
  }

  onNewReserveSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.reservData.propertyId = this.defaultPropertyId;
    this.reservData.custPhone = this.reservData.custPhone.toString();
    this.http.post('api/reservation/create'
      , this.reservData, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadInventories();
          
         
        this.toast('Success', 'Data saved successfully!', 'success');
         this.dialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
         
          console.log('api/reservation/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onReserveSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.reservData.custPhone = this.reservData.custPhone.toString();
    this.http.put('api/reservation/update?id=' + this.reservData.id
      , this.reservData, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {


          this.loadInventories();

          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;

          console.log('api/reservation/update', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  openReserveDelete(deleteReserveDialog: TemplateRef<any>) {

    this.dialogRef.close();
    this.dialogRef = this.dialogService.open(
      deleteReserveDialog,
      { context: { title: 'Delete Reservation' } });
  }

  deleteReservation() {


    this.loadingDelete = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/reservation/delete?id=' + this.reservData.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Reservation deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadInventories();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingDelete = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/reservation/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  loadRooms() {

    
    this.isLoadingRooms = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-room-list?id=' + this.defaultPropertyId
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
  onPropertySelected(event) {
   
    localStorage["default-reservation-property"] = this.defaultPropertyId;
    this.loadProperty();
  }

  loadProperty() {

    
    
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.defaultPropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
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

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}


class Property {

  id: string;
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

class Room {

  id: number;
  roomTypeId: string;
  propertyId: number;
  title: string;
  description: string;
  typeName: string;
}

class ReservData {

  id: number;  
  fromDate: string;
  toDate: string;
  roomId: string;
  propertyId: string;
  custName: string;
  custEmail: string;
  custPhone: string;  
  details: string;
  maxAvailableQty: number;
  noOfRooms: string;
  noOfGuests: number;
 
}
class BookingChannel {
  id: string;
  channelName: string;
}
class InvData {

  id: number;
  invDate: string;
  totalQty: number;
  bookedQty: number;
  roomId: string;
  propertyId: string;

}
