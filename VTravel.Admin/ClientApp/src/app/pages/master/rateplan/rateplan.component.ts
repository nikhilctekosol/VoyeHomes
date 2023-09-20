import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService } from '@nebular/theme';

@Component({
  selector: 'ngx-rateplan',
  templateUrl: './rateplan.component.html',
  styleUrls: ['./rateplan.component.scss']
})
export class RateplanComponent implements OnInit {

  isLoading = false;
  loadingSearch = false;
  loadingSave = false;
  loadingDelete = false;
  isLoadingRooms = false;
  isLoadingInventories = false;
  rateplans: any[];
  token: any;
  rateplan = new Rateplan();
  properties: any[];
  colors: any[];
  PropertyId: string = "0";
  ReferPlanId: string = "0";
  Percentage: string = "0";
  Amount: string = "0";
  property = new Property();
  dialogRef: any;
  loadingRatePlanSave = false;
  loadingplanDetailsSave = false;
  activerateplans: any[];
  inactiverateplans: any[];
  rateplandetails: PlanDetails[];
  referplandetails: PlanDetails[];
  rate: string = "0";
  planid: string = "0";
  nameedit: string ;
  coloredit: string ;
  constructor(private http: HttpClient, private authService: NbAuthService, private router: Router
    , private cd: ChangeDetectorRef, private r: ActivatedRoute, private dialogService: NbDialogService
    , private toastrService: NbToastrService) {

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();
    });
  }
  ngOnInit(): void {
    // if (localStorage["default-inventory-property"]) {
    //  this.PropertyId = localStorage["default-inventory-property"];
    // }
    this.loadProperties();
    this.rateplan.propertyId = '0';
  }

  private loadProperties() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list'
      , { headers: headers }).subscribe((res: any) => {


        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;

          }
        }

      },
        error => {
          console.log('api/property/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  onPropertySelected(event) {

    // localStorage["default-inventory-property"] = this.PropertyId;
    this.loadProperty();
    this.loadrateplans();
    this.loadactiverateplans();
    this.loadinactiverateplans();

  }
  loadProperty() {
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get?id=' + this.PropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.property = res.data;
        }

      },
        error => {

          console.log('api/property/get', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadColors() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-colors'
      , { headers: headers }).subscribe((res: any) => {


        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {

            this.colors = res.data;

          }
        }

      },
        error => {
          console.log('api/rateplan/get-colors', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadrateplans() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-list?id=' + this.PropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.rateplans = res.data;
        }

      },
        error => {

          console.log('api/rateplan/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadactiverateplans() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-activelist?id=' + this.PropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.activerateplans = res.data;
        }

      },
        error => {

          console.log('api/rateplan/get-activelist', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadinactiverateplans() {

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-inactivelist?id=' + this.PropertyId
      , { headers: headers }).subscribe((res: any) => {

        if (res.actionStatus == 'SUCCESS') {

          this.inactiverateplans = res.data;
        }

      },
        error => {

          console.log('api/rateplan/get-inactivelist', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  planedit(id, rateplanDialog: TemplateRef<any>, mode) {

    let rp;
    if (mode == 1) {
      rp = this.activerateplans.find(_ct => _ct.id == id);
    }
    else {
      rp = this.inactiverateplans.find(_ct => _ct.id == id);
    }

    this.rateplan.id = rp.id;
    this.rateplan.name = rp.name;
    this.rateplan.propertyId = this.PropertyId;
    this.rateplan.color = rp.color;


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-plan-details?id=' + id
      , { headers: headers }).subscribe((res: any) => {


        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.rateplandetails = res.data;
            this.planid = id;

            this.dialogRef = this.dialogService.open(
              rateplanDialog,
              { context: { title: rp.name, color : rp.color } });

          }
        }

      },
        error => {
          console.log('api/rateplan/get-plan-details', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  onreferplan(mode) {

    let rp = this.rateplans.find(_ct => _ct.id == this.ReferPlanId);

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/rateplan/get-plan-details?id=' + this.ReferPlanId
      , { headers: headers }).subscribe((res: any) => {


        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.rateplandetails = res.data;
            for (const item of this.rateplandetails) {
              // Update the 'value' property for each item
              item.id = 0;
              if (mode == 1) {
                this.Amount = "0";
                item.rate = (parseFloat(item.rate) * (100 +
                  parseFloat(this.Percentage == "" ? "0" : this.Percentage)) / 100).toString();
              }
              else if (mode == 2) {
                this.Percentage = "0";
                item.rate = (parseFloat(item.rate) + parseFloat(this.Amount == "" ? "0" : this.Amount)).toString();
              }
            }
          }
        }

      },
        error => {
          console.log('api/rateplan/get-plan-details', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  }

  referchange(mode) {
    if (mode == 1) {
      for (const item of this.rateplandetails) {
        item.rate = (parseFloat(item.rate) * (100 + parseFloat(this.Percentage)) / 100).toString();
      }
    }
    else {
      for (const item of this.rateplandetails) {
        item.rate = (parseFloat(item.rate) + parseFloat(this.Amount)).toString();
      }
    }
  }

  edit(rateDialog: TemplateRef<any>) {

    this.loadColors();
    this.dialogRef = this.dialogService.open(
      rateDialog,
      { context: { title: "Update Rate Plan" } });
  }

  ratechange(roomid, mealid, occid) {
    let plan = this.rateplandetails.find(_ct => _ct.roomid == roomid && _ct.mealid == mealid && _ct.occid == occid);

    if (plan != null) {
      plan.rate = this.rate;
    }
  }

  openRateNew(rateDialogNew: TemplateRef<any>) {
    if (this.PropertyId == "0") {
      this.toast('Error', 'Please select a property!', 'warning');
    }
    else {
      this.loadColors();
      this.rateplan.propertyId = this.PropertyId;
      this.dialogRef = this.dialogService.open(
        rateDialogNew,
        { context: { title: 'Add Rate Plan' } });
    }
  }

  onNewRatePlanSubmit() {
    this.loadingRatePlanSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/rateplan/create-rateplan'
      , this.rateplan, { headers: headers }).subscribe((res: any) => {
        this.loadingRatePlanSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingRatePlanSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
          this.loadactiverateplans();
          this.loadinactiverateplans();
        }
        else {

          this.loadingRatePlanSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingRatePlanSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/rateplan/create-rateplans', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });

  }

  onRatePlanSubmit() {

    this.loadingRatePlanSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/rateplan/update-rateplan'
      , this.rateplan, { headers: headers }).subscribe((res: any) => {
        this.loadingRatePlanSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingRatePlanSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
          this.loadactiverateplans();
          this.loadinactiverateplans();
        }
        else {

          this.loadingRatePlanSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingRatePlanSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/rateplan/update-rateplans', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  onPlanDetailsSubmit() {

    this.loadingplanDetailsSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.post('api/rateplan/create-rateplan-details?id=' + this.planid
      , this.rateplandetails, { headers: headers }).subscribe((res: any) => {

        this.loadingplanDetailsSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadingplanDetailsSave = false;
          this.toast('Success', 'Data saved successfully!', 'success');
          this.dialogRef.close();
        }
        else {

          this.loadingplanDetailsSave = false;
          this.toast('Error', 'Could not save data!', 'danger');


        }

      },
        error => {

          this.loadingplanDetailsSave = false;
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/rateplan/create-rateplan_details', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });
  }

  openRateplanDelete(id, deleteRateplanDialog: TemplateRef<any>) {
    this.rateplan = new Rateplan();

    this.rateplan.id = id;

    this.dialogRef = this.dialogService.open(
      deleteRateplanDialog,
      { context: { title: 'Delete Rateplan' } });
  }

  deleteRateplan() {
    this.loadingSave = true ;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/rateplan/delete?id=' + this.rateplan.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'Rateplan deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadactiverateplans();
          this.loadinactiverateplans();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/rateplan/delete', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}
class Rateplan {

  id: number;
  name: string ;
  propertyId: string ;
  color: string ;
  active: string ;
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

class PlanDetails {

  id: number;
  roomid: number;
  room: string ;
  mealid: number;
  meal: string ;
  occid: number;
  occupancy: string ;
  rate: string ;

}
