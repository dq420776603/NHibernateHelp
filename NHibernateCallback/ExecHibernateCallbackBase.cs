using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate;

namespace NHibernateHelp.NHibernateCallback
{
    public class ExecHibernateCallbackBase
    {
        public ExecHibernateCallbackBase()
        {
        }

        /// <summary>
        /// 执行所需Hql参数
        /// </summary>
        private List<ParamerInfo> paramerList = new List<ParamerInfo>();
        /// <summary>
        /// 执行所需Hql参数
        /// </summary>
        public List<ParamerInfo> ParamerList
        {
            get { return paramerList; }
            set { paramerList = value; }
        }
        /// <summary>
        /// 执行所需Hql集合参数
        /// </summary>
        private List<ParamerInfo> paramerIEnumerableList = null;
        /// <summary>
        /// 执行所需Hql集合参数
        /// </summary>
        public List<ParamerInfo> ParamerIEnumerableList
        {
            get
            {
                if (paramerIEnumerableList == null)
                {
                    paramerIEnumerableList = new List<ParamerInfo>();
                }
                return paramerIEnumerableList;
            }
            set { paramerIEnumerableList = value; }
        }
        /// <summary>
        /// 为创建出来的Query加参数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected IQuery setParameter(IQuery query)
        {
            if (ParamerList != null)
            {
                //p是Nhibernate用来顺序索引参数的
                for (int i = 0, p = 0; i < ParamerList.Count; i++)
                {
                    ParamerInfo paramInfo = ParamerList[i];
                    if (string.IsNullOrWhiteSpace(paramInfo.ParamerName))
                    {
                        if (paramInfo.ParamerType == null)
                        {
                            query.SetParameter(p, paramInfo.ParamerValue);
                        }
                        else
                        {
                            query.SetParameter(p, paramInfo.ParamerValue,
                                    paramInfo.ParamerType);
                        }
                        p++;
                    }
                    else
                    {
                        if (paramInfo.ParamerType == null)
                        {
                            query.SetParameter(paramInfo.ParamerName,
                                    paramInfo.ParamerValue);
                        }
                        else
                        {
                            query.SetParameter(paramInfo.ParamerName,
                                    paramInfo.ParamerValue,
                                    paramInfo.ParamerType);
                        }
                    }
                }
            }
            //设置集合参数
            if (ParamerIEnumerableList != null)
            {
                for (int i = 0; i < ParamerIEnumerableList.Count; i++)
                {
                    ParamerInfo paramInfo = ParamerIEnumerableList[i];
                    if (!string.IsNullOrWhiteSpace(paramInfo.ParamerName))
                    {
                        if (paramInfo.ParamerType == null)
                        {
                            query.SetParameterList(paramInfo.ParamerName,
                                    (IEnumerable)paramInfo.ParamerValue);
                        }
                        else
                        {
                            query.SetParameterList(paramInfo.ParamerName,
                                    (IEnumerable)paramInfo.ParamerValue,
                                    paramInfo.ParamerType);
                        }
                    }
                }
            }
            return query;
        }
    }
}
