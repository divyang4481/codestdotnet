# codestdotnet
Automatically exported from code.google.com/p/codestdotnet
一、基础应用

三句话访问数据库。话不多说，代码为证。以Access数据库为例：

DataManager dm = new OleDbManager(@”x:\data.mdb”);
dm.Open();
dm.Exec(”any SQL Command”);//执行SQL语句，返回受响应的行数 

以SQL Server数据库为例：

DataManager dm = new SQLManager(”serverIP”, “database”, “uid”, “pwd”);
dm.Open();
DataTable dt = dm.Select(”select * from table1″);//将查询结果导入DataTable中 

如果您指示想简单高效的访问数据库，实现上述代码，您需要下载Z.Data开源库。
二、高级应用

我们发现上面两段代码执行后都得到了一个数据管理器DataManager dm，无论是Access还是SqlServer，从而为数据库迁移提供了极大方便。 于是我们使用上述的dm对象，实现数据库的异步更新：

DataUpdater updater = dm.AllocateDataUpdater();
DataTable dt = updater.SelectWithUpdate(”select * from table1″);
//此处可以任意修改DataTable dt，增加、修改、删除行等。
updater.Update(dt); 

上面三句则实现了数据库的异步更新。

如果不是Access数据库也不是SqlServer数据库，如何连接？ 以自定义的连接字符串为例（MySQL）：

DataManager dm = new OleDbManager();
dm.ConnString = “Driver={mySQL};Server=localhost;Option=16834;Database=myDataBase;”;
dm.OpenByConnString()
这样我们还是可以得到一个通用的DataManager dm；进行各种操作。 

如果这些功能已经满足了您的要求，或者需要了解更多功能，请下载Z.Net开源库，参考附带的开发帮助。
三、原理

上述代码看起来很简洁，然而微软的东西向来以繁琐著称，相信接触过Win32API的朋友们都有深刻感触，于是我们不得不把很多功能进行封装，以便更方便的使用。下面是Icyplayer.Data的原理：
1、通用的数据库操作

DataManager类封装了一组抽象方法（更像一个接口），OleDbManager和SQLManager类则继承DataManager并实现了这些方法，于是通过DataManager的对象来访问不同数据库，只需修改连接代码，而其他访问代码都无需修改。
2、异步更新

我们需要两个对象：

//OleDbDataAdapter用来承载sql语句的执行结果，它可以把结果导出到DataTable或DataSet中
System.Data.OleDb.OleDbDataAdapter _dap;
//OleDbCommandBuilder必须关联一个OleDbDataAdapter，可以根据OleDbDataAdapter的查询结果自动生成更新sql语句
System.Data.OleDb.OleDbCommandBuilder _cmdb;

首先建立数据库连接：

System.Data.OleDb.OleDbConnection _conn;
_conn = new OleDbConnection();
_conn.ConnectionString = SQL_ConnectString;
_conn.Open();

然后，使cmdb关联到dap，并执行SQLCmd语句，把结果填充到dt中

DataTable dt = new DataTable();
_dap = new OleDbDataAdapter(SQLCmd, dataManager._conn);
_cmdb = new OleDbCommandBuilder(_dap);
_dap.Fill(dt);

此时，我们就可以修改dt中的内容 最后，提交更新

_dap.Update(dt); 

这就是让.net 帮我们自动生成sql语句的过程，简单吧~ 
