using DBHelp.SqlScriptDom;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NHibernateHelp.NHibernateCallback;
using Spring.Data.NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NHibernateHelp.Pager
{
    public class SqlDataPager
    {
        #region 数据操作部分
        /// <summary>
        /// 查询数据用的Spring.Data.NHibernate.HibernateTemplate
        /// </summary>
        public virtual HibernateTemplate Ht
        {
            get;
            set;
        }
        #endregion
        #region sql语句的固定部分
        /// <summary>
        /// 要执行的sql
        /// </summary>
        protected string sql = string.Empty;
        /// <summary>
        /// 要执行的sql
        /// </summary>
        public virtual string Sql
        {
            get
            {
                return sql;
            }
            set
            {
                sql = value;
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    string sqlCopy = sql.Replace(":", "@");
                    QuerySpecification queryExpression = MsSqlDom.GetSelectQuerySpecification(sqlCopy);
                    if (queryExpression != null)
                    {
                        FieldsSqlPart = sql.Substring(0, queryExpression.FromClause.StartOffset);
                        fromSqlPart = sql.Substring(queryExpression.FromClause.StartOffset, queryExpression.FromClause.FragmentLength);
                        if (queryExpression.OrderByClause != null && queryExpression.OrderByClause.OrderByElements != null
                            && queryExpression.OrderByClause.OrderByElements.Count > 0)
                        {
                            int orderElementStartIndex = queryExpression.OrderByClause.OrderByElements[0].StartOffset - queryExpression.OrderByClause.StartOffset;
                            orderSqlPart = sql.Substring(queryExpression.OrderByClause.OrderByElements[0].StartOffset, queryExpression.OrderByClause.FragmentLength - orderElementStartIndex);
                        }
                        string otherExpress1 = sql.Substring(queryExpression.WhereClause.StartOffset, queryExpression.OrderByClause.StartOffset - queryExpression.WhereClause.StartOffset);
                        otherExpress1 = otherExpress1.TrimStart();
                        if (otherExpress1.StartsWith("where", StringComparison.CurrentCultureIgnoreCase))
                        {
                            otherExpress1 = otherExpress1.Substring(5);
                            otherExpress1 = " and " + otherExpress1;
                        }
                        whereAndOtherExpress = otherExpress1;
                    }
                }
            }
        }
        /// <summary>
        /// 取数据的SQL中的Select部分
        /// </summary>
        private string fieldsSqlPart = string.Empty;
        /// <summary>
        /// 取数据的SQL中的Select部分
        /// </summary>
        public virtual string FieldsSqlPart
        {
            get
            {
                return fieldsSqlPart;
            }
            set
            {
                fieldsSqlPart = value ?? string.Empty;
                fieldsSqlPart = fieldsSqlPart.TrimStart();
            }
        }
        /// <summary>
        /// 取数据的SQL中的From部分
        /// </summary>
        private string fromSqlPart = string.Empty;
        /// <summary>
        /// 取数据的SQL中的From部分
        /// </summary>
        public virtual string FromSqlPart
        {
            get
            {
                return fromSqlPart;
            }
            set
            {
                fromSqlPart = value;
            }
        }
        #endregion
        #region 筛选条件
        /// <summary>
        /// 获得筛选where条件，fc表示的+数据权限部分
        /// </summary>
        /// <returns></returns>
        private string whereAndOtherExpress = string.Empty;
        /// <summary>
        /// 获得筛选where条件，fc表示的+数据权限部分
        /// </summary>
        /// <returns></returns>
        public virtual string WhereAndOtherExpress
        {
            get
            {
                string whereExpress1 = whereAndOtherExpress;
                if (!string.IsNullOrEmpty(whereExpress1))
                {
                    whereExpress1 = " where 1=1 " + whereExpress1;
                }
                return whereExpress1;
            }
            set
            {
                whereAndOtherExpress = value;
            }
        }
        /// <summary>
        /// 获得spring.net所需的参数
        /// </summary>
        private List<ParamerInfo> paramerList = null;
        /// <summary>
        /// 获得spring.net所需的参数
        /// </summary>
        public virtual List<ParamerInfo> ParamerList
        {
            get
            {
                if (paramerList == null || paramerList.Count <= 0)
                {
                    paramerList = new List<ParamerInfo>();
                }
                return paramerList;
            }
            set
            {
                paramerList = value;
            }
        }
        #endregion
        #region 排序部分
        /// <summary>
        /// 加上grid表示的，排序列
        /// </summary>
        private string orderSqlPart = string.Empty;
        /// <summary>
        /// 加上grid表示的，排序列
        /// </summary>
        public virtual string OrderSqlPart
        {
            get
            {
                string orderSqlPart1 = orderSqlPart;
                if (!string.IsNullOrWhiteSpace(orderSqlPart1))
                {
                    orderSqlPart1 = " order by " + orderSqlPart1;
                }
                return orderSqlPart1;
            }
            set
            {
                orderSqlPart = value;
            }
        }
        #endregion
        #region 算行数部分
        /// <summary>
        /// 如何获取Count的sql
        /// </summary>
        private bool simpleCount = false;
        /// <summary>
        /// 如何获取Count的sql
        /// </summary>
        public bool SimpleCount
        {
            get
            {
                return simpleCount;
            }

            set
            {
                simpleCount = value;
            }
        }
        #endregion

        /// <summary>
        /// 把sql和条件装载到queryEr，然后执行queryEr上的方法获得数据
        /// </summary>
        /// <param name="queryEr"></param>
        /// <returns></returns>
        public DataTable GetData(ExecSqlQuery queryEr)
        {
            //下面查询是导案板中的一条记录*一个卡主一条记录
            queryEr.Sql = FieldsSqlPart + " " + FromSqlPart + " " + WhereAndOtherExpress + " " + OrderSqlPart;
            queryEr.ParamerList.AddRange(this.ParamerList);
            IList listData = Ht.ExecuteFind(queryEr);
            DataTable dt = HashTableListToDataTable.ToDataTable(listData);
            return dt;
        }
        /// <summary>
        /// 根据当前筛选条件，获得不分页的数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllData()
        {
            ExecSqlQuery queryEr = new ExecSqlQuery();
            DataTable dt = GetData(queryEr);
            return dt;
        }
        #region 外部调用的方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public PagerData GetPagerData(int pageSize, int pageIndex)
        {
            SqlPager queryEr = new SqlPager();
            queryEr.FirstResult = pageSize * pageIndex;
            queryEr.MaxResults = pageSize;
            DataTable dt = GetData(queryEr);

            ExecSqlQuery countQuery = new ExecSqlQuery();
            if (SimpleCount)
            {
                countQuery.Sql = @"select count(1) as total " + FromSqlPart + " " + WhereAndOtherExpress;
            }
            else
            {
                countQuery.Sql = @"select count(1) as total " + string.Format(" from ({0}) t22 ", @"select 1 as id23 " + FromSqlPart + " " + WhereAndOtherExpress);
            }
            countQuery.ParamerList.AddRange(queryEr.ParamerList);
            IList listCountData = Ht.ExecuteFind(countQuery);
            int total = dt.Rows.Count;
            if (listCountData != null && listCountData.Count > 0)
            {
                int.TryParse(Convert.ToString((listCountData[0] as Hashtable)["total"]), out total);
            }
            PagerData returnData = new PagerData { Data = dt, Total = total };
            return returnData;
        }

        #endregion
    }
}
