/**
 * 用户信息
 */

export class UserInfo {
  Id: string; // 标识
  Psw: string; // 密码
  Name: string; // 用户名
  Tel: string; // 电话
  Mail: string; // 邮件
  NickName: string; // 昵称
  Status: number; // 状态

  constructor(options: {
    Id?: string;
    Psw?: string; // 密码
    Name?: string; // 用户名
    Tel?: string; // 电话
    Mail?: string; // 邮件
    NickName?: string; // 昵称
    Status?: number; // 状态
  } = {}) {
    this.Id = options.Id || '';
    this.Psw = options.Psw || '';
    this.Name = options.Name || '';
    this.Tel = options.Tel || '';
    this.Mail = options.Mail || '';
    this.NickName = options.NickName || '';
    this.Status = !!options.Status ? options.Status : 0;
  }
}
