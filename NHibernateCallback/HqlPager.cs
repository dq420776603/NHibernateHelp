using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Data.NHibernate;
using NHibernate;
using System.Collections;

namespace NHibernateHelp.NHibernateCallback
{
    public class HqlPager : ExecHqlQuery, IHibernateCallback
    {
        public HqlPager()
        {
        }
        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="hql"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        public HqlPager(String hql, int firstResult, int maxResults)
            : this()
        {
            this.Hql = hql;
            this.FirstResult = firstResult;
            this.maxResults = MaxResults;
        }
        /// <summary>
        /// 起始行
        /// </summary>
        private int? firstResult = null;
        /// <summary>
        /// 起始行
        /// </summary>
        public int? FirstResult
        {
            get { return firstResult; }
            set { firstResult = value; }
        }
        /// <summary>
        /// 读取多少行
        /// </summary>
        private int? maxResults = null;
        /// <summary>
        /// 读取多少行
        /// </summary>
        public int? MaxResults
        {
            get { return maxResults; }
            set { maxResults = value; }
        }

        object IHibernateCallback.DoInHibernate(NHibernate.ISession session)
        {
            IQuery query = this.CreateResultTransformerQuery(session);
            if (this.FirstResult != null)
            {
                query.SetFirstResult(Convert.ToInt32(this.FirstResult));
            }
            if (this.MaxResults != null)
            {
                query.SetMaxResults(Convert.ToInt32(this.MaxResults));
            }
            IList list = query.List();
            return list;
        }
    }
}
