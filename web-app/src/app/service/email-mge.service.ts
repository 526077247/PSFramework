/* Angular Version 6 below need to be deleted {providedIn: 'root'} */
/* angular */
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import {catchError} from 'rxjs/operators';
import {Result} from '../domain/result.domain';
/* owner */

@Injectable({
  providedIn: 'root'
})
export class EMailMgeSvr {
  baseurl: string;
  header: HttpHeaders;

  constructor(private httpClient: HttpClient) {
    this.baseurl = '/api/EMailMgeSvr.assx/';
    this.header = new HttpHeaders().append('Urlencode-Type', 'PS1801');
  }

  SendMail(address: string, content: string, subject: string, fromAlias: string, accountName: string, addressType?: number, replyToAddress?: boolean): Promise<boolean> {
    return new Promise((resolve, reject) => {
      let httpParams = new HttpParams()
        .append('address', address.toString())
        .append('content', content.toString())
        .append('subject', subject.toString())
        .append('fromAlias', fromAlias.toString())
        .append('accountName', accountName.toString());
      if (addressType !== undefined) {
        httpParams = httpParams.append('addressType', addressType.toString());
      }
      if (replyToAddress !== undefined) {
        httpParams = httpParams.append('replyToAddress', replyToAddress.toString());
      }
      this.httpClient.post<Result<boolean>>(this.baseurl + 'SendMail', httpParams, {headers: this.header}).subscribe(res => {
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
}
