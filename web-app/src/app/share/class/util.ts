import {FormControl, FormGroup} from '@angular/forms';

export class Util {
  /**
   * 将yyyy-MM-dd(T+)?hh:mm:ss 格式的字符串转化为时间对象 (兼容ios与android)
   */
  static strToDate(strDate: string): Date {
    if (strDate) {
      const arr = strDate.split(/[- :/T.]/);
      // tslint:disable-next-line:max-line-length
      return new Date(parseFloat(arr[0]), parseFloat(arr[1]) - 1, parseFloat(arr[2]), parseFloat(arr[3]), parseFloat(arr[4]), parseFloat(arr[5]));
    } else {
      return new Date();
    }
  }

  /**
   * 将时间对象简单格式化为yyyy-MM-dd
   */
  static dateToStr(date: Date): string {
    return Util.dateFormat(date, 'yyyy-MM-dd');
  }

  /**
   * 格式化时间对象为字符串
   */
  static dateFormat(date: Date, fmt: string): string {
    const o = {
      'M+': date.getMonth() + 1, // 月份
      'd+': date.getDate(), // 日
      'h+': date.getHours(), // 小时
      'm+': date.getMinutes(), // 分
      's+': date.getSeconds(), // 秒
      'q+': Math.floor((date.getMonth() + 3) / 3), // 季度
      S: date.getMilliseconds() // 毫秒
    };
    if (/(y+)/.test(fmt)) {
      fmt = fmt.replace(RegExp.$1, (date.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (const k in o) {
      if (new RegExp('(' + k + ')').test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1) ? (o[k]) : (('00' + o[k]).substr(('' + o[k]).length)));
      }
    }
    return fmt;
  }

  /**
   * 数组排序
   */
  static arraySort(arr: Array<any>, condition: string, isAsc?: boolean): Array<any> {
    arr.sort((a, b) => {
      if (a[condition] > b[condition]) {
        if (isAsc) {
          return -1;
        } else {
          return 1;
        }
      }
      if (a[condition] < b[condition]) {
        if (isAsc) {
          return;
        } else {
          return -1;
        }
      }
      return 0;
    });
    return arr;
  }

  /**
   * 深度转化json字符串为对象
   */
  static parseJson(str: string): object | string {
    if (!str) {
      return;
    }
    if (str.charAt(0) === '{') {
      try {
        const obj = JSON.parse(str);
        for (const key in obj) {
          if (obj.hasOwnProperty(key) && typeof obj[key] === 'string') {
            obj[key] = Util.parseJson(obj[key]);
          }
        }
        return obj;
      } catch (e) {
        return str;
      }
    } else if (str.charAt(0) === '[') {
      try {
        const obj = JSON.parse(str);
        for (let i = 0; i < obj.length; i++) {
          obj[i] = Util.parseJson(obj[i]);
        }
      } catch (e) {
        return str;
      }
    } else {
      return str;
    }
  }

  /**
   * 深拷贝对象
   */
  static deepClone(data: any): any {
    let o;
    let i;
    let ni;
    if (data instanceof Array) {
      o = [];
      for (i = 0, ni = data.length; i < ni; i++) {
        o.push(Util.deepClone(data[i]));
      }
      return o;
    } else if (data instanceof Object) {
      o = {};
      for (i in data) {
        if (data.hasOwnProperty(i)) {
          o[i] = Util.deepClone(data[i]);
        }
      }
      return o;
    } else {
      return data;
    }
  }

  static addUrlParam(url, param): string {
    return url + ((url.indexOf('?') >= 0) ? '&' : '?') + param;
  }

  static isObjectEmpty(obj) {
    if (Object.getOwnPropertyNames) {
      return Object.getOwnPropertyNames(obj).length === 0;
    }
    let k;
    for (k in obj) {
      if (obj.hasOwnProperty(k)) {
        return false;
      }
    }
    return true;
  }

  static uuid(): string {
    function S4() {
      // tslint:disable-next-line:no-bitwise
      return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    return (S4() + S4() + '-' + S4() + '-' + S4() + '-' + S4() + '-' + S4() + S4() + S4());
  }

  static markControlAsDirtyWithChildren(formGroup: FormGroup) {
    for (const controlName in formGroup.controls) {
      if (formGroup.controls.hasOwnProperty(controlName)) {
        const control = formGroup.get(controlName);
        if (control instanceof FormControl) {
          control.markAsDirty();
        } else if (control instanceof FormGroup) {
          Util.markControlAsDirtyWithChildren(control);
        }
      }
    }
  }

  static round(value: number, placesCount: number = 2) {
    if (!value) {
      return 0;
    }
    return Math.round(value * (10 ** placesCount)) / (10 ** placesCount);
  }

  static printDoc(html: string): void {
    const iframe = document.createElement('iframe') as HTMLIFrameElement;
    iframe.style.display = 'none';
    document.body.appendChild(iframe);
    iframe.contentDocument.body.innerHTML = html;
    setTimeout(() => {
      iframe.contentWindow.print();
      setTimeout(() => {
        iframe.remove();
      }, 5000);
    }, 2000);

  }
}
