import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NbDialogService, NbToastrService, NbComponentStatus, NbDialogContainerComponent  } from '@nebular/theme';
import { CalendarOptions, FullCalendarComponent, EventClickArg, EventApi, DateSelectArg } from '@fullcalendar/angular';


@Component({
  selector: 'inventory',
  templateUrl: './inventory.component.html',
  styleUrls: ['./inventory.component.scss'],
})


export class InventoryComponent implements OnInit {

  dateRange: any;
  channels: any[];
  deleteDialogRef: any;
  invDialogRef: any;
  dialogRef: any;
  dayList: any[]=[];
  inventories: any[];
  rooms: any[];
  room = new Room();
  reservData = new ReservData();
  invData = new InvData();
  defaultPropertyId: string = "0";
  isLoading = false;
  loadingSearch = false;
  loadingSave = false;
  loadingDelete = false;
  isLoadingRooms = false;
  isLoadingInventories = false;
  properties: any[];
 
  token: any;
  property = new Property();
 
 




  

  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {


    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();


    });

   

  }

  ngOnInit() {

    if (localStorage["default-inventory-property"]) {
      this.defaultPropertyId= localStorage["default-inventory-property"];
    }


    var date1 = new Date();

    var date2 = new Date();
    date2.setDate(date2.getDate() + 15);

    this.dateRange = { start: date1, end: date2 };

    this.loadBookingChannels();
    this.loadProperties();
    this.loadProperty();
   

  

  }

 

  loadInventories() {

    this.dayList = [];
    //console.log(this.dateRange);

    var dateStr1 = this.dateRange.start.toString().split('GMT', 1)[0];
    var dateStr2 = this.dateRange.end.toString().split('GMT', 1)[0];

    var date1 = new Date(dateStr1);
    var date2 = new Date(dateStr2);


    this.inventories = [];
   
    this.isLoadingInventories = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/inventory/get-list?propertyId=' + this.property.id
      + '&dateFrom=' + this.formatDate(date1) + '&dateTo=' + this.formatDate(date2)
      , { headers: headers }).subscribe((res: any) => {

        //console.log(JSON.stringify(res));
        
        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.inventories = res.data;

          

          }

          var date1Tmp = new Date(dateStr1);

          var i = 0;
          while (date1Tmp <= date2) {

            //console.log(date1Tmp);

            var dayObj = { "dayObj": date1Tmp.toDateString() };
            this.dayList.push(dayObj);
            date1Tmp.setDate(date1Tmp.getDate() + 1);

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

  formatDate(date) {
  var d = new Date(date),
    month = '' + (d.getMonth() + 1),
    day = '' + d.getDate(),
    year = d.getFullYear();

  if (month.length < 2)
    month = '0' + month;
  if (day.length < 2)
    day = '0' + day;

  return [year, month, day].join('-');
}

  getInv(invDate, roomId) {

   var inv= {
      "totalQty": 0,
     "bookedQty": 0,
     "price": 0,
     "extraBedPrice": 0,
     "childPrice":0
    }

    var invDateA = this.formatDate((new Date(invDate)).toString());
    for (var i = 0; i < this.inventories.length; i++) {

      var invDateB = this.formatDate((new Date(this.inventories[i].invDate)).toString());

      if ((invDateA == invDateB) && (roomId == this.inventories[i].roomId)) {
        
        inv.totalQty = this.inventories[i].totalQty;
        inv.bookedQty = this.inventories[i].bookedQty;
        inv.price = this.inventories[i].price;
        inv.extraBedPrice = this.inventories[i].extraBedPrice;
        inv.childPrice = this.inventories[i].childPrice;
       
      }
    }

    return inv;
  }

  

  onInvChange(mode,value, invDate, roomId) {

    
    this.loadingSave = true;

    this.invData = new InvData();
    if (mode == 'QTY') {
      this.invData.totalQty = parseInt(value);
    } else if (mode == 'PRICE') {
      this.invData.price = parseFloat(value);
    } else if (mode == 'EXTRA_BED_PRICE') {
      this.invData.extraBedPrice = parseFloat(value);
    } else if (mode == 'CHILD_PRICE') {
      this.invData.childPrice = parseFloat(value);
    }
    
    this.invData.invDate = this.formatDate((new Date(invDate)).toString());
    this.invData.roomId = roomId.toString();
    this.invData.propertyId = this.property.id.toString();
    this.invData.mode = mode;

    //console.log(JSON.stringify(this.invData));

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    
    this.http.post('api/inventory/create'
      , this.invData, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {


          this.loadInventories();

          this.toast('Success', 'Data saved successfully!', 'success');
          
        }
        else {

          this.loadingSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;

          console.log('api/inventory/create', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onNewReserveSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

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

  onInvSubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.invData.invDate = this.formatDate((new Date(this.invData.fromDate)).toString());
    this.invData.fromDate = this.formatDate((new Date(this.invData.fromDate)).toString());
    this.invData.toDate = this.formatDate((new Date(this.invData.toDate)).toString());
    this.invData.propertyId = this.property.id.toString();

    //console.log(JSON.stringify(this.invData));

    this.http.post('api/inventory/create-bulk'
      , this.invData, { headers: headers }).subscribe((res: any) => {
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

          console.log('api/inventory/create-bulk', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  openInv(invDialog: TemplateRef<any>,roomId) {

   
    this.invData = new InvData();
    this.invData.roomId = roomId.toString();
    this.dialogRef = this.dialogService.open(
      invDialog,
      { context: { title: 'Bulk Update' } });
  }

  deleteInventory() {


    this.loadingDelete = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/inventory/delete?id=' + this.reservData.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingDelete = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Inventory deleted successfully!', 'success');
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

          console.log('api/inventory/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  loadRooms() {

    
    this.isLoadingRooms = true;
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
  onPropertySelected(event) {
   
    localStorage["default-inventory-property"] = this.defaultPropertyId;
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
          this.loadInventories();
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  private loadProperties() {

    this.loadingSave = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.loadingSave = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;
           
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

  private loadBookingChannels() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/bookingchannel/get-list'
      , { headers: headers }).subscribe((res: any) => {

       
        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.channels = res.data;
            //console.log(JSON.stringify(this.channels));
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/bookingchannel/get-list', error)
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
  bookingChannelId: string;
  details: string;

}
class BookingChannel {
  id: string;
  channelName: string;
}
class InvData {

  id: number;
  invDate: string;
  fromDate: string;
  toDate: string;
  totalQty: number;
  bookedQty: number;
  roomId: string;
  propertyId: string;
  mode: string;
  price: number;
  extraBedPrice: number;
  childPrice: number;
  
}
