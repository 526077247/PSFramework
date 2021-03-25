# sso
.netcore 3.0、ibatis、castle、mysql、redis
## 1.项目结构
### service.core
    核心框架，已发布到Nuget，可搜索Service.Core-PsFramework安装使用
### sql
    创建数据库参考文件
### sso.service
    包含核心业务代码的项目
### sso
    发布web服务相关配置项目
### web-app
    web前端示例项目
## 2.配置方法
1. Nuget上搜索Service.Core-PsFramework可安装框架
     dotnet add package Service.Core-PsFramework --version 1.0.15
     Install-Package Service.Core-PsFramework -Version 1.0.15
2. appsettings.json中添加如下
```
  {
   "serviceCore": {
    "daoFile": "config/dao.config",
    "servicesFile": "config/Components.xml"
   },
   ...
  }
```
3. 在发布web服务项目中添加文件夹config和文件config/Components.xml(castle配置文件)，config/dao.config、config/providers.config、config/SqlMap.config(ibatis配置文件)并设置生成时复制到输出目录。配置内容参考本项目。
    
## 3.服务类
### 3.1 定义服务接口
```
using service.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace 命名空间
{
    /// <summary>
    /// 测试服务
    /// </summary>
    public interface ITestSvr : IAppServiceBase 
    {

        /// <summary>
        /// 获取数值
        /// </summary>
        /// <param name="i">数值</param>
        /// <returns>数值</returns>
        [PublishMethod]//发布web指定可访问
        int GetNum(int i);

    }
}
```
### 3.2 定义服务实现类

```

using IBatisNet.DataAccess;
using service.core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace 命名空间
{
    /// <summary>
    /// 测试服务
    /// </summary>
    public class TestSvr : AppServiceBase, ITestSvr 
    {
        #region 服务描述：测试服务

        public TestSvr() : base()
        {

        }

        #endregion


        #region ITestSvr函数

        /// <summary>
        /// 获取数值
        /// </summary>
        /// <param name="i">数值</param>
        /// <returns>数值</returns>
        public int GetNum(int i){
          return i;
        }
        
        #endregion
    }
}
```
### 3.3 配置文件添加组件

在 "/你的发布web配置项目/config/Components.xml" 文件中添加如下组件

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <components>
   ...
    <component id="自定义服务名" type="service.core.ProxyService, service.core" service="service.core.IProxyService, service.core">
      <parameters>
        <service>命名空间.接口名, 程序集</service>
        <url>url地址</url>
      </parameters>
    </component>
   ...
 </components>
</configuration>
```
### 3.4 发布web服务
如果要发布web服务可通过http访问，需添加配置文件

在"/你的发布web配置项目/wwwroot/xx路径"下添加"自定义服务名.json"

```
{
  "SvrID": "自定义服务名",
  "IntfName": "命名空间.接口类型名称",
  "IntfAssembly": "程序集"
}
```

### 3.5 在其他服务中使用
```
private readonly ITestSvr svr = null;
public TestSvr2()
{
    svr = ServiceManager.GetService<ITestSvr>("接口名称");
}
```

## 4.使用缓存（如redis）
### 4.1 在配置文件中添加
```
{
 ...
  "Caches": {
    "LoginResult": {
      "CacheID": "LoginResult",
      "Type": "Redis",
      "Host": "127.0.0.1",
      "Port": 6379,
      "lifeTime": {
        "hours": 12,
        "min": 0,
        "seconds": 0
      },
      "Size": 256
    }
  },
 ...
}
```

### 4.2 使用方式
```
private readonly ICacheManager cacheManager = null;
private readonly ICacheMgeSvr _CacheMgeSvr = null;
public TestSvr()
{
    cacheManager = (ICacheManager)ServiceManager.GetService(typeof(ICacheManager));
    _CacheMgeSvr = cacheManager.GetCache("LoginResult");
}
```

## 5.使用动态代理远程服务

### 5.1 在配置文件中创建 (推荐使用)
* 注意：需要定义和远程服务一样的接口

* 配置文件添加组件
在 "/你的web发布配置项目/config/Components.xml" 文件中添加如下组件
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <components>
   ...
    <component id="自定义服务名" type="service.core.ProxyService, service.core" service="service.core.IProxyService, service.core">
      <parameters>
        <service>命名空间.接口名, 程序集</service>
        <url>url地址</url>
      </parameters>
    </component>
   ...
 </components>
</configuration>
```

* 在其他服务中使用方法同3.5

* 然后就可以像调用本地服务一样调用远程接口了

### 5.2 在代码中创建
* 也需要定义和远程服务一样的接口
```
    T p = DynServerFactory.CreateServer<T>("服务url", "json");
```

