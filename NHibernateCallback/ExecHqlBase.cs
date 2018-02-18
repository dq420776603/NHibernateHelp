using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace NHibernateHelp.NHibernateCallback
{
    public class ExecHqlBase : ExecHibernateCallbackBase
    {
        public ExecHqlBase()
        {
        }

        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="hql"></param>
        public ExecHqlBase(String hql)
            : this()
        {
            this.Hql = hql;
        }

        /// <summary>
        /// 带参数的构造方法，方便用此类
        /// </summary>
        /// <param name="hql"></param>
        /// <param name="paramerList"></param>
        public ExecHqlBase(String hql, IEnumerable<ParamerInfo> paramerList) :
            this(hql)
        {
            if (this.ParamerList == null)
            {
                this.ParamerList = new List<ParamerInfo>();
            }
            this.ParamerList.AddRange(paramerList);
        }
        /// <summary>
        /// 要执行的hql语句
        /// </summary>
        private String hql = string.Empty;
        /// <summary>
        /// 要执行的hql语句
        /// </summary>
        public String Hql
        {
            get { return hql; }
            set { hql = value; }
        }

        /// <summary>
        /// 根据本类实例的属性设置创建查询
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected IQuery createQuery(ISession session)
        {
            IQuery query = session.CreateQuery(Hql);
            query = setParameter(query);
            return query;
        }
    }
}
