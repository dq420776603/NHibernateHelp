using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Data.NHibernate;
using NHibernate;
using System.Collections;
using NHibernate.Transform;

namespace NHibernateHelp.NHibernateCallback
{

    public class ExecHqlQuery : ExecHqlBase, IHibernateCallback
    {
        public ExecHqlQuery()
        {
        }
        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="hql"></param>
        public ExecHqlQuery(String hql) :
            this()
        {
            Hql = hql;
        }
        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="hql"></param>
        /// <param name="paramerList"></param>
        public ExecHqlQuery(String hql, List<ParamerInfo> paramerList) :
            this(hql)
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
            IQuery query = this.createQuery(session);
            //query = query.SetResultTransformer(this.ResultTransformer);
            return query;
        }

        object IHibernateCallback.DoInHibernate(ISession session)
        {
            IQuery query = this.CreateResultTransformerQuery(session);
            IList resultList = query.List();
            return resultList;
        }
    }
}
