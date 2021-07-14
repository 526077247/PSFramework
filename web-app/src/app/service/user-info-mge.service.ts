/* Angular Version 6 below need to be deleted {providedIn: 'root'} */
/* angular */
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {catchError} from 'rxjs/operators';
import {Result} from '../domain/result.domain';
/* owner */
import {LoginResult} from '../domain/loginresult.domain';
import {UserInfo} from '../domain/userinfo.domain';

@Injectable({
  providedIn: 'root'
})
export class UserInfoMgeSvr {
  baseurl: string;
  header: HttpHeaders;

  constructor(private httpClient: HttpClient) {
    this.baseurl = '/Service/UserInfoMgeSvr.rsfs/';
    this.header = new HttpHeaders().append('Urlencode-Type', 'PS1801');
  }

  RegisterUser(name: string, nickName: string, psw: string): Promise<LoginResult> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('name', name)
        .append('nickName', nickName)
        .append('psw', psw);
      this.httpClient.post<Result<LoginResult>>(this.baseurl + 'RegisterUser', httpParams, {headers: this.header}).subscribe(res => {
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

  CheckUserName(name: string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('name', name);
      this.httpClient.post<Result<boolean>>(this.baseurl + 'CheckUserName', httpParams, {headers: this.header}).subscribe(res => {
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

  CheckEMail(mail: string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('mail', mail.toString());
      this.httpClient.post<Result<boolean>>(this.baseurl + 'CheckEMail', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0){
          reject(res.msg);
        } else {
          resolve(res.data as boolean);
        }
      }, err => {
        reject(err);
      });
    });
  }

  RegisterUserDetails(userInfo: UserInfo): Promise<LoginResult> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('userInfo', JSON.stringify(userInfo));
      this.httpClient.post<Result<LoginResult>>(this.baseurl + 'RegisterUserDetails', httpParams, {headers: this.header}).subscribe(res => {
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

  ChangePsw(user: string, oldPsw: string, newPsw: string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('user', user)
        .append('oldPsw', oldPsw)
        .append('newPsw', newPsw);
      this.httpClient.post<Result<boolean>>(this.baseurl + 'ChangePsw', httpParams, {headers: this.header}).subscribe(res => {
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

  GetUser(token: string): Promise<UserInfo> {
    return new Promise((resolve, reject) => {
      const httpParams = new HttpParams()
        .append('token', token);
      this.httpClient.post<Result<UserInfo>>(this.baseurl + 'GetUser', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(new UserInfo(res.data));
        }
      }, err => {
        reject(err);
      });
    });
  }

  UpdateUser(token: string, nickName?: string, tel?: string, mail?: string): Promise<UserInfo> {
    return new Promise((resolve, reject) => {
      let httpParams = new HttpParams()
        .append('token', token);
      if (nickName !== undefined) {
        httpParams = httpParams.append('nickName', nickName.toString());
      }
      if (tel !== undefined) {
        httpParams = httpParams.append('tel', tel.toString());
      }
      if (mail !== undefined) {
        httpParams = httpParams.append('mail', mail.toString());
      }
      this.httpClient.post<Result<UserInfo>>(this.baseurl + 'UpdateUser', httpParams, {headers: this.header}).subscribe(res => {
        if (res.code !== 0) {
          reject(res.msg);
        } else {
          resolve(new UserInfo(res.data));
        }
      }, err => {
        reject(err);
      });
    });
  }

  ForgetPswForEMail(name:string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      let httpParams = new HttpParams()
        .append('name', name);
      this.httpClient.post<Result<boolean>>(this.baseurl + 'ForgetPswForEMail', httpParams, {headers: this.header}).subscribe(res => {
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

  ChangePswByTicket(ticket:string, newPsw:string): Promise<boolean> {
    return new Promise((resolve, reject) => {
      let httpParams = new HttpParams()
        .append('ticket', ticket)
        .append('newPsw', newPsw);
      this.httpClient.post<Result<boolean>>(this.baseurl + 'ChangePswByTicket', httpParams, {headers: this.header}).subscribe(res => {
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
}
