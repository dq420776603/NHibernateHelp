using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace NHibernateHelp.NHibernateCallback
{
    public class ExecSqlBase : ExecHibernateCallbackBase
    {
        /// <summary>
        /// 要执行的sql语句
        /// </summary>
        private String sql = string.Empty;
        /// <summary>
        /// 要执行的sql语句
        /// </summary>
        public String Sql
        {
            get { return sql; }
            set { sql = value; }
        }
        /// <summary>
        /// 根据本类实例的属性设置创建查询
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected IQuery createSQLQuery(ISession session)
        {
            IQuery query = session.CreateSQLQuery(Sql);
            query = setParameter(query);
            return query;
        }
    }
}
