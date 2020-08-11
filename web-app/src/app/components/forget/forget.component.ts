import { Component, OnInit } from '@angular/core';
import { UserInfoMgeSvr } from 'src/app/service/user-info-mge.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-forget',
  templateUrl: './forget.component.html',
  styleUrls: ['./forget.component.less']
})
export class ForgetComponent implements OnInit {

  public name:string;
  constructor(
    private userInfoMgeSvr:UserInfoMgeSvr,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
  }

  showEmail():void{
    this.userInfoMgeSvr.ForgetPswForEMail(this.name).then(res=>{
      this.snackBar.open('发送'+(res?'成功，请进入邮箱查收':'失败，请稍后再试'), '', {duration: 2000});
    })
  }
}
