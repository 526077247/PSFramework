import {Injectable} from '@angular/core';


import {CookieUtil} from '../share/class/cookie';
import {LoginResult} from '../domain/loginresult.domain';
import {ActivatedRoute, Router} from '@angular/router';
import {LoginMgeSvr} from './login-mge.service';
import {MatSnackBar} from "@angular/material/snack-bar";



const SESSION_NAME = '_Myt_Session_Token_Info_';

@Injectable({
  providedIn: 'root'
})
export class LoginAuthorService {

  cookie = new CookieUtil();

  constructor(
    private authorizeSvr: LoginMgeSvr,
    private router: Router,
    private snackBar: MatSnackBar,
    private activatedRoute: ActivatedRoute,
  ) {
  }

  get userInfo() {
    const res = new LoginResult(JSON.parse(this.cookie.cookie(SESSION_NAME) || '{}'));
    if (!res.Token) {
      this.router.navigateByUrl('/login?redirect_url=' + encodeURIComponent(window.location.pathname));
    }
    return res;
  }

  get token(): string {
    return this.userInfo.Token;
  }

  get isLogin(): boolean {
    const res = new LoginResult(JSON.parse(this.cookie.cookie(SESSION_NAME) || '{}'));
    return !!res.Token;
  }

  login(username: string, password: string): void {
    this.authorizeSvr.Login(username, password).then(res => {
      if (res.Token) {
        this.cookie.cookie(SESSION_NAME, JSON.stringify(res), {path: '/', expires: res.Effective});
        this.activatedRoute.queryParams.subscribe(queryParam => {
          if (window.location.pathname === '/login') {
            if (!!queryParam.redirect_url) {
              this.router.navigateByUrl(decodeURIComponent(queryParam.redirect_url));
            } else {
              this.router.navigateByUrl('/');
            }
          }else{
            location.href= 'https://mayuntao.xyz/';
          }
          this.snackBar.open('登录成功', '', {duration: 2000});
        });
      } else {
        this.snackBar.open('登录失败', '', {duration: 2000});
      }
    }, err => {
      this.snackBar.open('登录失败', '', {duration: 2000});
    });
  }

  logout(): Promise<boolean> {
    return new Promise((resolve, reject) => {
      this.authorizeSvr.Logout(this.token).then(res => {
        if (res) {
          this.cookie.cookie(SESSION_NAME, res, {path: '/', expires: 0});
          this.router.navigateByUrl('/login');
          resolve(true);
        } else {
          resolve(false);
        }
      }, err => {
        reject(err);
      });
    });

  }

}
