# sso
技术栈 .netcore 3.0、ibatis、castle、mysql、redis
## 1.项目结构
### 
## 2.配置方法
## 3.服务类
## 4.使用缓存
## 5.使用动态代理远程服务
```
[PublishMethod]
public string Post()
{
    IUserInfoMgeSvr p = DynServerFactory.CreateServer<IUserInfoMgeSvr>("http://127.0.0.1:8081/api/UserInfoMgeSvr.proxy", "stream");

    //执行方法看效果   

    //string result = HttpPostHelper.PostStream<string>("http://127.0.0.1:8081/api/UserInfoMgeSvr.proxy/GetVersion", "");
    string result = p.GetVersion();
    return result;
}
```
