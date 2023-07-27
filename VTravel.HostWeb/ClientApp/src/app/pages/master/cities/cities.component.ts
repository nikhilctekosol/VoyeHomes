import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService} from '@nebular/theme';



@Component({
  selector: 'cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss'],
})


export class CitiesComponent implements OnInit  {

 
  isLoading = false;
  loadingSave = false; 
  cities: any[];
  countries: any[];
  states: any[];
  token: any;   
  city = new City();
  dialogRef: any;

  constructor(private http: HttpClient, private authService: NbAuthService, 
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {

  
    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

      
    });
   
  }

  ngOnInit() {

    this.loadCities();
    this.loadCountries();
  
  }

  loadCountries() {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/location/get-country-list'
      , { headers: headers }).subscribe((res: any) => {



        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.countries = res.data;
            this.loadStates('IN');
          }
        }

      },
        error => {

          console.log('api/location/get-country-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  loadStates(countryCode) {


    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/location/get-state-list?countryCode='+countryCode
      , { headers: headers }).subscribe((res: any) => {



        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.states = res.data;

          }
        }

      },
        error => {

          console.log('api/location/get-state-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  private loadCities() {

    this.isLoading = true;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/location/get-city-list-all'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.cities = res.data;
           
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/location/get-city-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });
  
  }

  openCityNew(cityDialogNew: TemplateRef<any>) {

    this.city = new City();
    this.dialogRef = this.dialogService.open(
      cityDialogNew,
      { context: { title: 'Add City' } });


  }

  openCity(cityDialog: TemplateRef<any>, id) {

    let ct = this.cities.find(_ct => _ct.id == id);

    this.city = new City();

    this.city.id = id;
    
    this.city.cityName = ct.cityName;
    this.city.cityCode = ct.cityCode;
    this.city.stateCode = ct.stateCode;
    this.city.countryCode = ct.countryCode;

    this.dialogRef = this.dialogService.open(
      cityDialog,
      { context: { title: 'Update City' } });


  }

  openCityDelete(deleteCityDialog: TemplateRef<any>, id) {

    this.city = new City();

    this.city.id = id;

    this.dialogRef = this.dialogService.open(
      deleteCityDialog,
      { context: { title: 'Delete City' } });
  }

  onNewCitySubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    
    this.http.post('api/location/create-city'
      , this.city, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadCities();
          this.loadingSave = false;
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
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/location/create-city', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  onCitySubmit() {

    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.put('api/location/update-city?id=' + this.city.id
      , this.city, { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.loadCities();
          this.loadingSave = false;
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
          this.toast('Error', 'Could not save data!', 'danger');

          console.log('api/location/update-city', error);
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }

        });




  }

  deleteCity() {


    this.loadingSave = true;

    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");

    this.http.delete('api/location/delete-city?id=' + this.city.id
      , { headers: headers }).subscribe((res: any) => {
        this.loadingSave = false;

        if (res.actionStatus === 'SUCCESS') {

          this.toast('Success', 'City deleted successfully!', 'success');
          this.dialogRef.close();
          this.loadCities();

        }
        else {


          this.toast('Error', 'Could not delete!', 'danger');


        }

      },
        error => {

          this.loadingSave = false;
          this.toast('Error', 'Could not delete!', 'danger');

          console.log('api/location/delete-city', error);
          if (error.status === 401) {
            this.router.navigate(['/auth/login']);
          }

        });

  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }
}



class City {
  id: number;
  cityName: string;
  cityCode: string;
  stateCode: string;
  countryCode: string;
}


