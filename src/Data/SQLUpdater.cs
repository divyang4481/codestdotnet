/*******************************************************************************
 * * 版权所有(C) CODEST.ORG. 本软件遵循GPL协议。
 * * 文件名称：SQLUpdater.cs
 * * 作　　者：ZhaoYu(email@zhaoyu.me) <http://www.zhaoyu.me/>
 * * 创建日期：2009年08月24日 18时00分58秒
 * * 文件标识：9FA64F08-E37B-4579-A18B-DEA743CFED03
 * * 内容摘要：
 * *******************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace codest.Data
{
    /// <summary>
    /// SQL Server操作数据库更新器
    /// </summary>
    public class SQLUpdater : DataUpdater
    {
        #region 成员变量
        /// <summary>
        /// 当前更新器所使用的数据库管理器
        /// </summary>
        protected SQLManager dataManager;
        /// <summary>
        /// 当有更新操作时，用来保存数据信息
        /// </summary>
        protected SqlDataAdapter _dap; 
        /// <summary>
        /// 当有更新操作时，配合DataAdapter使用
        /// </summary>
        protected SqlCommandBuilder _cmdb;
        #endregion

        #region 接口封装

        #endregion

        #region 构造/析构函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">容器中唯一的ID</param>
        /// <param name="manager">数据库管理器</param>
        public SQLUpdater(int id, SQLManager manager)
            : base(id)
        {
            dataManager = manager;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~SQLUpdater()
        {
            this.Release();
        }
        #endregion

        #region  protected override void Dispose(bool disposing)
        /// <summary>
        /// 释放由当前对象控制的所有资源
        /// </summary>
        /// <param name="disposing">显式调用</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                //释放托管资源
            }
            //释放非托管资源
            dataManager = null;
            _cmdb = null;
            _dap = null;
            base.Dispose(disposing);
        }
        #endregion

        #region public override DataTable SelectWithUpdate(string SQLCmd)
        /// <summary>
        /// 使对象将进入修改模式状态
        /// 用户可以修改返回结果，包括删除、修改和增加行
        /// 再调用Update(DataTable)进行更新操作
        /// 此后，对象将退出修改模式状态
        /// </summary>
        /// <param name="SQLCmd">SQL语句</param>
        /// <returns>查询响应结果</returns>
        public override DataTable SelectWithUpdate(string SQLCmd)
        {
            dataManager.execNum++;
            System.Data.DataTable dt = new DataTable();
            _dap = new SqlDataAdapter(SQLCmd, dataManager._conn);
            _cmdb = new SqlCommandBuilder(_dap);
            _dap.Fill(dt);
            return dt;
        }
        #endregion

        #region public override DataTable InsertMode(string TableName)
        /// <summary>
        /// 使对象进入修改模式
        /// 用户可以在返回表结构的DataTable中添加数据
        /// 再调用Update(DataTable)进行更新操作
        /// 此后，对象将退出修改模式状态
        /// </summary>
        /// <param name="TableName">需要插入的表名称</param>
        /// <returns>要插入目标表的结构</returns>
        public override DataTable InsertMode(string TableName)
        {
            dataManager.execNum++;
            System.Data.DataTable dt = new DataTable();
            _dap = new SqlDataAdapter("select * from [" + TableName + "] where 1=0", dataManager._conn);
            _cmdb = new SqlCommandBuilder(_dap);
            _dap.Fill(dt);
            return dt;
        }
        #endregion

        #region public override void Update(System.Data.DataTable DataTableSource)

        /// <summary>
        /// 关闭修改模式,并根据DataTable进行更新操作
        /// </summary>
        /// <param name="DataTableSource">要提交的数据表</param>
        public override void Update(System.Data.DataTable DataTableSource)
        {
            dataManager.execNum++;
            _dap.Update(DataTableSource);
            DecideRelease();
        }
        #endregion

        #region  public override void Release()
        /// <summary>
        /// 释放当前更新器。
        /// 如果AutoRelease=true，则调用Update()后会自动调用该方法。
        /// </summary>
        public override void Release()
        {
            dataManager.ReleaseDataUpdater(this);
        }
        #endregion
    }
}
