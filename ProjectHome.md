# 简介 #
微软的.Net Framework为我们提供了非常强大功能，但有时候我们不得不将一些功能进行再次封装，以方便开发和维护。本类库封装了一些常用的.Net类库，简化了使用方法，使代码更加简洁，提高编程效率。
# 类库功能 #
## 命名空间codest ##
  * 提供一种基类，实现IDisposable, ICloneable接口
  * 提供一种基类，可以使子类自动拥有序列化、反序列话功能（XML及二进制） http://code.google.com/p/codestdotnet/wiki/Serialize
## codest.Data ##
  * 所有类型数据库封装了统一的操作接口和基类，使数据库迁移更加方便;
  * 引入一种更新器模式，在多线程环境下可以异步访问数据库
  * DataManager:数据管理器基类，封装了统一的方法
  * DataUpdater:数据库更新器的基类，提供更新操作的规范
  * 更多数据库支持:Access、SQL Server、MySQL。。。全部统一接口
  * Wiki:http://code.google.com/p/codestdotnet/wiki/Z_Data
## codest.Net ##
  * ASP.NET实现文件上传及管理
  * TCP异步访问控制，包括服务端多线程控制及客户端方法
  * 模拟HTTP GET&POST通信，能正确使用Session和Cookies
  * 动态调用Web Service
## codest.Encode ##
  * DES加密解密
  * DSA数字签名