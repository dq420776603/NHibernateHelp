using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernateHelp.Pager
{
    /// <summary>
    /// 返回的分页数据结构
    /// </summary>
    public class PagerData
    {
        /// <summary>
        /// 当页数据
        /// </summary>
        public virtual object Data
        {
            get;
            set;
        }
        /// <summary>
        /// 所有页总共数据行数
        /// </summary>
        public virtual int Total
        {
            get;
            set;
        }
    }
}
