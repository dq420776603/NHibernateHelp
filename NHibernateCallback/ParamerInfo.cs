using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Type;

namespace NHibernateHelp.NHibernateCallback
{
    public class ParamerInfo
    {
        /// <summary>
        /// 参数值
        /// </summary>
        private Object paramerValue = null;
        /// <summary>
        /// 参数值
        /// </summary>
        public Object ParamerValue
        {
            get { return paramerValue; }
            set { paramerValue = value; }
        }
        /// <summary>
        /// 参数名
        /// </summary>
        private String paramerName = null;
        /// <summary>
        /// 参数名
        /// </summary>
        public String ParamerName
        {
            get { return paramerName; }
            set { paramerName = value; }
        }
        /// <summary>
        /// 参数类型
        /// </summary>
        private IType paramerType = null;
        /// <summary>
        /// 参数类型
        /// </summary>
        public IType ParamerType
        {
            get { return paramerType; }
            set { paramerType = value; }
        }
    }
}
