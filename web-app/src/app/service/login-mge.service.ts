/* Angular Version 6 below need to be deleted {providedIn: 'root'} */
/* angular */
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {catchError} from 'rxjs/operators';
import {Result} from '../domain/result.domain';
/* owner */
import {LoginResult} from '../domain/loginresult.domain';

@Injectable({
  providedIn: 'root'
})
export class LoginMgeSvr {
  baseurl: string;
  header: HttpHeaders;

  constructor(private httpClient: HttpClient) {
    this.baseurl = '/Service/LoginMgeSvr.rsfs/';
    this.header = new HttpHeaders().append('Urlencode-Type', 'PS1801');
  }

  Login(user: string, psw: string): Promise<LoginResult> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('user', user)
        .append('psw', psw);
      this.httpClient.post<Result<LoginResult>>(this.baseurl + 'Login', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(new LoginResult(res.data));
        }
      }, err => {
        reject(err);
      });
    });
  }

  Refresh(token: string): Promise<LoginResult> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('token', token);
      this.httpClient.post<Result<LoginResult>>(this.baseurl + 'Refresh', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(new LoginResult(res.data));
        }
      }, err => {
        reject(err);
      });
    });
  }

  Logout(token: string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('token', token);
      this.httpClient.post<Result<boolean>>(this.baseurl + 'Logout', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(res.data as boolean);
        }
      }, err => {
        reject(err);
      });
    });
  }

  GetAuthorizationCode(user: string, psw: string): Promise<string> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('user', user)
        .append('psw', psw);
      this.httpClient.post<Result<string>>(this.baseurl + 'GetAuthorizationCode', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(res.data);
        }
      }, err => {
        reject(err);
      });
    });
  }

  LoginByCode(code: string): Promise<LoginResult> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('code', code);
      this.httpClient.post<Result<LoginResult>>(this.baseurl + 'LoginByCode', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(new LoginResult(res.data));
        }
      }, err => {
        reject(err);
      });
    });
  }

  GetLoginInfo(token: string): Promise<LoginResult> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('token', token);
      this.httpClient.post<Result<LoginResult>>(this.baseurl + 'GetLoginInfo', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(new LoginResult(res.data));
        }
      }, err => {
        reject(err);
      });
    });
  }
}
