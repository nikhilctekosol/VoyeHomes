import { Component, OnInit, ChangeDetectorRef, TemplateRef, ContentChild, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import {  NbToastrService, NbComponentStatus, NbDialogContainerComponent  } from '@nebular/theme';
import { CalendarOptions, FullCalendarComponent, EventClickArg, EventApi, DateSelectArg } from '@fullcalendar/angular';


@Component({
  selector: 'enquiry',
  templateUrl: './enquiries.component.html',
  styleUrls: ['./enquiries.component.scss'],
})


export class EnquiriesComponent implements OnInit {


  isLoading = false;
  enquiries: any[];     
  loadingSave = false;
  token: any;
  page_no = 0;
  page_no_start = 0;
  page_count = 0;
  page_count_show = 10;
  rows_count = 20;
  total_count = 0;
  enquiry_status= 'ALL';
  

  constructor(private http: HttpClient, private authService: NbAuthService,
    private cd: ChangeDetectorRef, private router: Router,
    private r: ActivatedRoute) {


    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();

    });

  }

  ngOnInit() {
         
    this.loadEnquiries();
    
  }

  loadPage(page_no) {
    if (page_no != this.page_no) {
      this.page_no = page_no;
      this.loadEnquiries();
    }
   
  }

  loadNext() {
    this.page_no += 1;
    this.loadEnquiries();
  }

  loadPrev() {
    this.page_no -= 1;
    this.loadEnquiries();
  }

  paginate() {
    this.page_count = Math.floor(this.total_count / this.rows_count);
    if (this.total_count / this.rows_count > 0) {
      this.page_count += 1;
    }

    this.page_count_show = this.page_count > this.page_count_show ? this.page_count_show : this.page_count;

    if (this.page_no >= this.page_count_show) {
      if ((this.page_count - (this.page_no + 1)) <= 5) {
        this.page_no_start = this.page_count - this.page_count_show - 1;
      } else {
        this.page_no_start = this.page_no - (this.page_count_show / 2);
      }

    } else {
      this.page_no_start = 0;
    }      
  }

  loadEnquiries() {

   
    this.isLoading = true;

    //this.enquiries = [];

    
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/enquiry/get-list?enquiry_status=' + this.enquiry_status
      + '&page_no=' + this.page_no + '&rows_count=' + this.rows_count
      , { headers: headers }).subscribe((res: any) => {

        //console.log(JSON.stringify(res));

        if (res.actionStatus == 'SUCCESS') {
        
            this.enquiries = res.data.record_list;
            this.total_count = res.data.record_count[0].total_count;

          this.paginate();  
                    
        }
        this.isLoading = false;
      },
        error => {

          this.isLoading = false;
          console.log('api/enquiry/get-list', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }

  resetPageValues() {
    this.page_no = 0;
    this.page_no_start = 0;
    this.page_count = 0;
    this.page_count_show = 10;
    this.rows_count = 20;
    this.total_count = 0;
  }
  onStatusSelected(event) {
    //console.log(this.enquiry_status);
    /*localStorage["default-reservation-property"] = this.defaultPropertyId;*/
    this.resetPageValues();
    this.loadEnquiries();
  }
}


