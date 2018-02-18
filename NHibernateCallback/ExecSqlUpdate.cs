using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Data.NHibernate;
using NHibernate;
using NHibernate.Transform;

namespace NHibernateHelp.NHibernateCallback
{
    public class ExecSqlUpdate : ExecSqlBase, IHibernateCallback
    {
        public ExecSqlUpdate()
        {
        }

        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="sql"></param>
        public ExecSqlUpdate(String sql) :
            this()
        {
            this.Sql = sql;
        }

        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramerList"></param>
        public ExecSqlUpdate(String sql, List<ParamerInfo> paramerList) :
            this(sql)
        {
            this.ParamerList = paramerList;
        }
        /// <summary>
        /// 最后返回的格式，默认以Map返回
        /// </summary>
        private IResultTransformer resultTransformer = Transformers.AliasToEntityMap;
        /// <summary>
        /// 最后返回的格式，默认以Map返回
        /// </summary>
        public IResultTransformer ResultTransformer
        {
            get { return resultTransformer; }
            set { resultTransformer = value; }
        }
        /// <summary>
        /// 创建带着指定输出格式的IQuery
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected virtual IQuery CreateResultTransformerQuery(ISession session)
        {
            IQuery query = this.createSQLQuery(session);
            query = query.SetResultTransformer(this.ResultTransformer);
            return query;
        }

        object IHibernateCallback.DoInHibernate(ISession session)
        {
            IQuery query = this.CreateResultTransformerQuery(session);
            Object resultList = query.List();
            return resultList;
        }
    }
}
