import { Component, OnDestroy, OnInit } from '@angular/core';
import { NbMediaBreakpointsService, NbMenuService, NbSidebarService, NbThemeService } from '@nebular/theme';

import { UserData } from '../../../@core/data/users';
import { LayoutService } from '../../../@core/utils';
import { map, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { NbAuthService, NbAuthJWTToken, NbAuthToken } from '@nebular/auth';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'ngx-header',
  styleUrls: ['./header.component.scss'],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit, OnDestroy {

  private destroy$: Subject<void> = new Subject<void>();
  userPictureOnly: boolean = false;
  user: any;
  properties: any[];
  themes = [
    {
      value: 'default',
      name: 'Light',
    },
    {
      value: 'dark',
      name: 'Dark',
    },
    {
      value: 'cosmic',
      name: 'Cosmic',
    },
    {
      value: 'corporate',
      name: 'Corporate',
    },
  ];
  token: any;
  defaultPropertyId: string = "0";
  currentTheme = 'default';

  userMenu = [ { title: 'Profile' }, { title: 'Log out' } ];
    

  constructor(private sidebarService: NbSidebarService,
              private menuService: NbMenuService,
              private themeService: NbThemeService,
              private userService: UserData,
    private layoutService: LayoutService,
    private authService: NbAuthService,
    private breakpointService: NbMediaBreakpointsService,
    private router: Router, private http: HttpClient,) {
    
    this.authService.onTokenChange()
      .subscribe((token: NbAuthJWTToken) => {
        if (token.isValid()) {
          this.user = token.getPayload();
          //console.log(JSON.stringify(token));
        }

      });

    this.authService.getToken().subscribe((tokenData: NbAuthToken) => {
      this.token = tokenData.getValue();


    });
  }

  ngOnInit() {
    this.currentTheme = this.themeService.currentTheme;

    

    if (localStorage["default-reservation-property"]) {
      this.defaultPropertyId = localStorage["default-reservation-property"];
      
    }

    //console.log(this.defaultPropertyId);
    //console.log(JSON.stringify(this.user));

    //this.userService.getUsers()
    //  .pipe(takeUntil(this.destroy$))
    //  .subscribe((users: any) => this.user = users.nick);

    const { xl } = this.breakpointService.getBreakpointsMap();
    this.themeService.onMediaQueryChange()
      .pipe(
        map(([, currentBreakpoint]) => currentBreakpoint.width < xl),
        takeUntil(this.destroy$),
      )
      .subscribe((isLessThanXl: boolean) => this.userPictureOnly = isLessThanXl);

    this.themeService.onThemeChange()
      .pipe(
        map(({ name }) => name),
        takeUntil(this.destroy$),
      )
      .subscribe(themeName => this.currentTheme = themeName);

    this.menuService.onItemClick().subscribe((event) => {
      this.onItemSelection(event.item.title);
    })

    this.loadProperties();
  }

  onPropertySelected($event) {

    
    localStorage["default-reservation-property"] = this.defaultPropertyId;
    location.reload();
  }

  private loadProperties() {

    
    let headers = new HttpHeaders().set("Authorization", "Bearer " +
      this.token).set("Content-Type", "application/json");
    this.http.get('api/property/get-list-sorted-by-name'
      , { headers: headers }).subscribe((res: any) => {

       

        if (res.actionStatus == 'SUCCESS') {
          if (res.data.length > 0) {
            this.properties = res.data;

          }
        }

      },
        error => {
          
          console.log('api/property/get-list-sorted-by-name', error)
          if (error.status === 401) {
            this.router.navigate(['auth/login']);
          }
        });

  }


  onItemSelection(title) {
    if (title === 'Log out') {
      this.router.navigate(['/auth/logout']);
    } else if (title === 'Profile') {
      // go to profile
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  changeTheme(themeName: string) {
    this.themeService.changeTheme(themeName);
  }

  toggleSidebar(): boolean {
    this.sidebarService.toggle(true, 'menu-sidebar');
    this.layoutService.changeLayoutSize();

    return false;
  }

  navigateHome() {
    this.menuService.navigateHome();
    return false;
  }
}
