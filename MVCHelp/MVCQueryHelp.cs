using DBHelp.SqlScriptDom;
using Extend.MVCHelp;
using Extend.SqlScriptDom;
using Extend.SqlScriptDom.SqlPartN;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NHibernateHelp.NHibernateCallback;
using Spring.Data.NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NHibernateHelp.MVCHelp
{
    /// <summary>
    /// EasyUI的Grid用的返回的分页数据结构
    /// </summary>
    public class MVCQueryHelp
    {
        #region 数据操作所需外部传入对象
        /// <summary>
        /// 查询数据用的Spring.Data.NHibernate.HibernateTemplate
        /// </summary>
        public virtual HibernateTemplate Ht
        {
            get;
            set;
        }
        #endregion
        #region 获取数据部分
        #region GetAllData部分
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="queryEr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList GetAllData(object args, string sql)
        {
            ExecSqlQuery queryEr = MVCNHibernateHelp.BuildSqlQuery(args, sql);
            SqlPart sqlPart = MVCNHibernateHelp.RefactorToSqlPart(sql);
            return GetData(queryEr, sqlPart);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="queryEr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList GetAllData(MVCRequest request, object args, string sql)
        {
            ExecSqlQuery queryEr = MVCNHibernateHelp.BuildSqlQuery(args, sql);
            SqlPart sqlPart = MVCNHibernateHelp.RefactorToSqlPart(request, sql);
            return GetData(queryEr, sqlPart);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="queryEr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList GetAllData(IDictionary<string, object> args, string sql)
        {
            ExecSqlQuery queryEr = MVCNHibernateHelp.BuildSqlQuery(args, sql);
            SqlPart sqlPart = MVCNHibernateHelp.RefactorToSqlPart(sql);
            return GetData(queryEr, sqlPart);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="queryEr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList GetAllData(MVCRequest request, IDictionary<string, object> args, string sql)
        {
            ExecSqlQuery queryEr = MVCNHibernateHelp.BuildSqlQuery(args, sql);
            SqlPart sqlPart = MVCNHibernateHelp.RefactorToSqlPart(request, sql);
            return GetData(queryEr, sqlPart);
        }
        #endregion
        /// <summary>
        /// 获取EasyUIRequest对应的分页数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public MVCPagerData GetPagerData(MVCRequest request, object args, string sql)
        {
            ExecSqlQuery queryEr = MVCNHibernateHelp.BuildPager(request, args, sql);
            queryEr.Sql = sql;
            return GetPagerData(request, queryEr);
        }
        /// <summary>
        /// 获取EasyUIRequest对应的分页数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public MVCPagerData GetPagerData(MVCRequest request, IDictionary<string, object> args, string sql)
        {
            ExecSqlQuery queryEr = MVCNHibernateHelp.BuildPager(request, args, sql);
            queryEr.Sql = sql;
            return GetPagerData(request, queryEr);
        }
        /// <summary>
        /// 获取EasyUIRequest对应的分页数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="queryEr"></param>
        /// <returns></returns>
        public MVCPagerData GetPagerData(MVCRequest request, ExecSqlQuery queryEr)
        {
            #region 数据部分
            string sql = queryEr.Sql;
            SqlPart sqlPart = MVCNHibernateHelp.RefactorToSqlPart(request, sql);
            IList listData = GetData(queryEr, sqlPart);
            #endregion
            #region count行数部分
            //默认行数
            int total = GetCount(queryEr, sqlPart);
            #endregion
            return new MVCPagerData { rows = listData, total = total };
        }
        #region 内部调用方法
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="queryEr"></param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public IList GetData(ExecSqlQuery queryEr, SqlPart sqlPart)
        {
            queryEr.Sql = SqlPartHelp.ConvertToSqlString(sqlPart);
            IList listData = Ht.ExecuteFind(queryEr);
            return listData;
        }
        /// <summary>
        /// 获取行数
        /// </summary>
        /// <param name="queryEr"></param>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public int GetCount(ExecSqlQuery queryEr, SqlPart sqlPart)
        {
            int total = 0;
            ExecSqlQuery countQuery = new ExecSqlQuery();
            countQuery.Sql = SqlPartHelp.GetCountSql(sqlPart);
            countQuery.ParamerList.AddRange(queryEr.ParamerList);
            IList listCountData = Ht.ExecuteFind(countQuery);
            if (listCountData != null && listCountData.Count > 0)
            {
                int.TryParse(Convert.ToString((listCountData[0] as Hashtable)["total"]), out total);
            }
            return total;
        }
        #endregion
        #endregion
    }
}
