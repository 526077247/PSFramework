import {Component, OnInit} from '@angular/core';
import {UserInfo} from 'src/app/domain/userinfo.domain';
import {UserInfoMgeSvr} from '../../service/user-info-mge.service';
import {CookieUtil} from '../../share/class/cookie';

import {ActivatedRoute, Router} from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

const SESSION_NAME = '_Myt_Session_Token_Info_';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.less']
})
export class RegisterComponent implements OnInit {

  public step: number;
  public userInfo: UserInfo = new UserInfo();
  public psw2: string;
  public isCheck = false;
  public pattern = new RegExp('[`~!@#%$^&*()=|{}\':;\',\\[\\].<>《》/?~！@#￥……&*（）——|{}【】‘；：”“\'。，、？]');
  public patternZ = /[a-z]/i;
  public patternN = /[0-9]/;
  public isCheckPass = 0;
  public isCheckUserName = 0;
  public isCheckNickName = 0;
  public isCheckEmail = 0;
  public isCheckPsw = 0;
  public isCheckPsw2 = 0;
  cookie = new CookieUtil();

  public get userNameErr(): string {
    switch (this.isCheckUserName) {
      case 0:
        return;
      case 1:
        return;
      case 2:
        return '用户名重复';
      case 3:
        return '长度不足9位';
      case 4:
        return '不能包含特殊字符';
      case 5:
        return '必须字母开头';
      case 6:
        return '邮箱已被注册';
    }
  }

  public get userPswErr(): string {
    switch (this.isCheckPsw) {
      case 0:
        return;
      case 1:
        return;
      case 2:
        return '密码必须同时包含数字和字母';
      case 3:
        return '长度不足6位';
      case 4:
        return '密码必须同时包含数字和字母';
    }
  }

  constructor(
    private userInfoMgeSvr: UserInfoMgeSvr,
    private snackBar: MatSnackBar,
    private router: Router,
    private activatedRoute: ActivatedRoute,
  ) {
  }

  ngOnInit() {


  }

  public startCheck(): void {
    if (this.isCheck) {
      if (this.step >= 100) {
        this.isCheckPass = 1;
      } else {
        this.step = 0;
      }
    }
  }

  public checkUserName(): void {
    if (!!this.userInfo.Name && this.userInfo.Name.length > 8) {
      if (this.pattern.test(this.userInfo.Name)) {
        this.isCheckUserName = 4;
        return;
      }
      if (!this.patternZ.test(this.userInfo.Name[0])) {
        this.isCheckUserName = 5;
        return;
      }
      this.userInfoMgeSvr.CheckUserName(this.userInfo.Name).then(res => {
        this.isCheckUserName = res ? 1 : 2;
      });
    } else {
      this.isCheckUserName = 3;
    }
  }


  public checkNickName(): void {
    if (!!this.userInfo.NickName && this.userInfo.NickName.length > 1) {
      this.isCheckNickName = this.pattern.test(this.userInfo.NickName) ? 2 : 1;
    } else {
      this.isCheckNickName = 3;
    }
  }

  public checkPsw(): void {
    if (!!this.userInfo.Psw && this.userInfo.Psw.length >= 6) {
      if (!this.patternZ.test(this.userInfo.Psw)) {
        this.isCheckPsw = 2;
        return;
      }
      if (!this.patternN.test(this.userInfo.Psw)) {
        this.isCheckPsw = 4;
        return;
      }
      this.isCheckPsw = 1;
    } else {
      this.isCheckPsw = 3;
    }
  }
  public checkEmail():void{
    if (!!this.userInfo.Mail && this.userInfo.Mail.length < 20) {
      this.userInfoMgeSvr.CheckEMail(this.userInfo.Mail).then(res => {
        this.isCheckEmail = res ? 1 : 6;
      });
    } else {
      this.isCheckEmail = 3;
    }
    
  }
  public checkPsw2(): void {
    this.isCheckPsw2 = this.userInfo.Psw === this.psw2 ? 1 : 2;
  }

  public registe(): void {
    if (this.isCheckPsw === 0) {
      this.checkPsw();
    }
    if (this.isCheckPsw2 === 0) {
      this.checkPsw2();
    }
    if (this.isCheckPass === 0) {
      this.isCheckPass = 2;
    }
    if (this.isCheckNickName === 0) {
      this.checkNickName();
    }
    if (this.isCheckUserName === 0) {
      this.checkUserName();
    }
    if (this.isCheckEmail === 0) {
      this.checkEmail();
    }
    if (this.isCheckPsw === 1 && this.isCheckPsw2 === 1 && this.isCheckNickName === 1&& this.isCheckEmail === 1 && this.isCheckUserName === 1) {
      this.userInfoMgeSvr.RegisterUserDetails(this.userInfo).then(res => {
        if (res.Token) {
          this.snackBar.open('注册成功', '', {duration: 2000});
          this.activatedRoute.queryParams.subscribe(queryParam => {
            if (!!queryParam.redirect_url) {
              window.location.href = decodeURIComponent(queryParam.redirect_url);
            } else {
              this.cookie.cookie(SESSION_NAME, JSON.stringify(res), {path: '/', expires: res.Effective});
              this.router.navigateByUrl('/');
            }
          })
        }
      }, err => {
        this.snackBar.open('注册失败', '', {duration: 2000});
      });
    }
  }

}
