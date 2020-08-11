import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserInfoMgeSvr } from 'src/app/service/user-info-mge.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-reset',
  templateUrl: './reset.component.html',
  styleUrls: ['./reset.component.less']
})
export class ResetComponent implements OnInit {

  public newpsw: string;
  public newpsw2: string;
  private ticket: string;
  public pattern = new RegExp('[`~!@#%$^&*()=|{}\':;\',\\[\\].<>《》/?~！@#￥……&*（）——|{}【】‘；：”“\'。，、？]');
  public patternZ = /[a-z]/i;
  public patternN = /[0-9]/;
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private userInfoMgeSvr: UserInfoMgeSvr,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(queryParam => {
      if (!!queryParam.ticket) {
        this.ticket = queryParam.ticket;
      } else {
        this.router.navigateByUrl('/');
      }
    });
  }

  resetPsw(): void {
    if (!!this.newpsw && this.newpsw.length >= 6) {
      if (!this.patternZ.test(this.newpsw)) {
        this.snackBar.open('密码必须同时包含数字和字母', '', { duration: 2000 });
        return;
      }
      if (!this.patternN.test(this.newpsw)) {
        this.snackBar.open('密码必须同时包含数字和字母', '', { duration: 2000 });
        return;
      }
      if (this.newpsw !== this.newpsw2) {
        this.snackBar.open('两次输入不一致', '', { duration: 2000 });
      } else {
        this.userInfoMgeSvr.ChangePswByTicket(this.ticket, this.newpsw).then(res => {
          this.snackBar.open('修改' + (res ? '成功' : '失败'), '', { duration: 2000 });
        })
      }
    } else {
      this.snackBar.open('长度不足6位', '', { duration: 2000 });
    }
    
  }

}
