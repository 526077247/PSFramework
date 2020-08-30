import { Component, OnInit } from '@angular/core';
import {LoginAuthorService} from '../../service/login-author.service';
import {Title} from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { LoginMgeSvr } from 'src/app/service/login-mge.service';
import {Util} from 'src/app/share/class/util';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent implements OnInit {
  public username = '';
  public password = '';
  public redirectUrl:string;
  public type=0;
  constructor(
    private loginSvr: LoginAuthorService,
    private loginMgeSvr:LoginMgeSvr,
    private titleService: Title,
    private activatedRoute: ActivatedRoute,
  ) {
  }

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(queryParam => {
      console.log(queryParam.step);
      if (!!queryParam.step&&queryParam.step=='A1') {

        this.redirectUrl=decodeURIComponent(queryParam.redirectUrl);
        this.type = 1;
      } else {
        this.type=0;
      }
    });
  }

  /**
   * 登录
   */
  public login(): void {
    if(this.type==0){
      this.loginSvr.login(this.username, this.password);
    }else if(this.type==1){
      this.loginToGetCode();
    }
  }

  /**
   * 登录
   */
  public loginToGetCode(): void {
    this.loginMgeSvr.GetAuthorizationCode(this.username, this.password).then(res=>{
      if(!!res){
        this.redirectUrl=this.redirectUrl.replace('code=','code='+res);
        let vs = this.redirectUrl.split('&redirectUrl=');
        this.redirectUrl= Util.addUrlParam(vs[0],'redirectUrl='+encodeURIComponent(vs[1]));
        location.href= this.redirectUrl;
      }else{

      }
    });
  }
  /*
  * 按键触发
  * */
  public selectParaChange(e) {
    // tslint:disable-next-line: deprecation
    const keycode = window.event ? e.keyCode : e.which;
    if (keycode === 13) {// 回车键
        this.login();
    }
  }
}
