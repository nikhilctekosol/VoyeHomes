import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { NbDialogService, NbComponentStatus, NbToastrService } from '@nebular/theme';

@Component({
  selector: 'ngx-tds',
  templateUrl: './tds.component.html',
  styleUrls: ['./tds.component.scss']
})
export class TdsComponent implements OnInit {

  isLoading = false;
  loadingSave = false;
  tdslist: any[] = [];
  token: any;
  tds = new TDS();
  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute, private dialogService: NbDialogService, private toastrService: NbToastrService) {


    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();


    });

  }

  ngOnInit(): void {
    this.loadTDS();
  }

  private loadTDS() {

    this.isLoading = true ;
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/tds/get-list'
      , { headers: headers }).subscribe((res: any) => {

        this.isLoading = false;

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.tdslist = res.data;
          }
        }

      },
        error => {
          this.isLoading = false;
          console.log('api/tds/get-list', error)
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

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
  }

  onTDSSubmit() {
    if (this.validateFields()) {

      this.loadingSave = true ;

      this.tds.effective = this.formatDate(this.tds.effective);

      let headers = new HttpHeaders().set("Authorization", "Bearer " +
        this.token).set("Content-Type", "application/json");

      this.http.post('api/tds/create'
        , this.tds, { headers: headers }).subscribe((res: any) => {
          this.loadingSave = false;

          if (res.actionStatus === 'SUCCESS') {

            this.loadTDS();
            this.loadingSave = false;
            this.toast('Success', 'Data saved successfully!', 'success');
            this.tds = new TDS();
          }
          else {

            this.loadingSave = false;
            this.toast('Error', 'Could not save data!', 'danger');

          }

        },
          error => {
            this.loadingSave = false;
            this.toast('Error', 'Could not save data!', 'danger');

            console.log('api/tds/create', error);
            if (error.status === 401) {
              this.router.navigate(['auth/login']);
            }
          });
    }
  }

  validateFields(): boolean {
    // Check if the effective from date is in the future (optional)
    const currentDate = new Date();
    if (new Date(this.tds.effective).setHours(0, 0, 0, 0) < currentDate.setHours(0, 0, 0, 0)) {
      this.toast('Error', 'Effective From date should not be less than current date!', 'danger');
      this.tds.effective = '';
      return false;
    }


    if (this.tds.percentage > 100) {
      this.toast('Error', 'Percentage should not be greater than 100!', 'danger');
      this.tds.percentage = 0;
      return false;
    }

    // All checks passed, the dates are valid
    return true ;
  }

  toast(title, message, status: NbComponentStatus) {
    this.toastrService.show(title, message, { status });
  }

}

class TDS {
  id: number;
  ownershiptype: string ;
  percentage: number;
  effective: string ;
}
