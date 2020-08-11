/**
 * 登陆信息
 */
export class LoginResult {
  Token: string; // 会话标识
  NickName: string; // 昵称
  Name: string; // 账号名
  LoginTime: string; // 上线时间
  Effective: number; // 有效时间
  constructor(options: {
    Token?: string;
    NickName?: string;
    Name?: string;
    LoginTime?: string;
    Effective?: number;
  } = {}) {
    this.Token = options.Token || '';
    this.NickName = options.NickName || '';
    this.Name = options.Name || '';
    this.LoginTime = options.LoginTime || '';
    this.Effective = !options.Effective ? 0 : Number.parseFloat(options.Effective.toString());
  }
}

