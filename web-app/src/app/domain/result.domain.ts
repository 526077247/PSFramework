


/**
 * 返回值
 */
export class Result<T>  {
  code: number; // 代码
  msg: string; // 消息
  data: T; // 返回数据

  constructor(options: {
    code?: number;
    msg?: string;
    data?: T;
  } = {}) {
    this.code = !options.code ? 0 : Number.parseFloat(options.code.toString());
    this.msg = options.msg || '';
    this.data = options.data || null;
  }
}

