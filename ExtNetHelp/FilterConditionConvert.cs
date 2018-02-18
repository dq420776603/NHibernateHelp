using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Ext.Net;
using System.Collections;
using NHibernateHelp.NHibernateCallback;
using NHibernate.Type;
using Newtonsoft.Json.Linq;

namespace Extend.ExtNetHelp
{
    /// <summary>
    /// 帮助由FilterCondition获得表达式
    /// </summary>
    public class FilterConditionConvert
    {
        #region 各个属性的默认值
        /// <summary>
        /// 默认的字符串连接用字符
        /// </summary>
        private static string defaultStrJoinSign = "+";
        /// <summary>
        /// 默认的字符串连接用字符
        /// </summary>
        public static string DefaultStrJoinSign
        {
            get { return FilterConditionConvert.defaultStrJoinSign; }
            set { FilterConditionConvert.defaultStrJoinSign = value; }
        }
        /// <summary>
        /// 参数名格式，对于Nhibernate为:{0}，对于不同种数据库可能不一样
        /// </summary>
        private static string defaultParamerNameFormat = ":{0}";
        /// <summary>
        /// 参数名格式，对于Nhibernate为:{0}，对于不同种数据库可能不一样
        /// </summary>
        public static string DefaultParamerNameFormat
        {
            get { return defaultParamerNameFormat; }
            set { defaultParamerNameFormat = value; }
        }
        /// <summary>        
        /// 默认的字符型字段表达式格式，主要是要加isnull
        /// </summary>
        private static string DefaultStringFieldFormat = @" isnull({0},'') ";
        /// <summary>        
        /// 如果，碰到字符串like的话，如何匹配
        /// </summary>
        private static string defaultLikeFormat = string.Format("'%' {0} {1} {0} '%'", DefaultStrJoinSign, DefaultStringFieldFormat);
        /// <summary>
        /// 如果，碰到字符串like的话，如何匹配
        /// </summary>
        public static string DefaultLikeFormat
        {
            get { return defaultLikeFormat; }
            set { defaultLikeFormat = value; }
        }
        /// <summary>        
        /// 默认的集合参数中参数名格式
        /// </summary>
        private static string defaultListSingleParamerNameFormat = @"{0}_{1}";
        /// <summary>
        /// 默认的集合参数中参数名格式
        /// </summary>
        public static string DefaultListSingleParamerNameFormat
        {
            get { return defaultListSingleParamerNameFormat; }
            set { defaultListSingleParamerNameFormat = value; }
        }
        /// <summary>        
        /// SQL中用的默认的集合参数中参数名格式
        /// </summary>
        private static string defaultListSingleParamerNameFormatPart = string.Format(DefaultParamerNameFormat, DefaultListSingleParamerNameFormat);
        /// <summary>
        /// SQL中用的默认的集合参数中参数名格式
        /// </summary>
        public static string DefaultListSingleParamerNameFormatPart
        {
            get { return defaultListSingleParamerNameFormatPart; }
            set { defaultListSingleParamerNameFormatPart = value; }
        }
        #endregion

        #region 各个属性值
        /// <summary>
        /// JTokenType枚举值对应程序类型的对照表
        /// </summary>
        private Dictionary<JTokenType, Type> valueToTypeHash = null;
        /// <summary>
        /// JTokenType枚举值对应程序类型的对照表
        /// </summary>
        public Dictionary<JTokenType, Type> ValueToTypeHash
        {
            get
            {
                if (valueToTypeHash == null)
                {
                    valueToTypeHash = new Dictionary<JTokenType, Type>();
                    JTokenType[] jtkList = new JTokenType[] { JTokenType.Object , JTokenType.Integer , JTokenType.Float ,
                        JTokenType.String ,JTokenType.Boolean , JTokenType.Date , JTokenType.Bytes , JTokenType.Guid , JTokenType.TimeSpan };
                    foreach (JTokenType jtokenType in jtkList)
                    {
                        valueToTypeHash[jtokenType] = Type.GetType("System." + jtokenType);
                    }
                    #region 这些是上文对应不对的
                    valueToTypeHash[JTokenType.Integer] = typeof(System.Int32);
                    valueToTypeHash[JTokenType.Float] = typeof(System.Decimal);
                    valueToTypeHash[JTokenType.Date] = typeof(System.DateTime);
                    #endregion
                    #region 折叠里面这些，NHibernate不支持
                    //JTokenType.  Uri ,
                    // JTokenType. Raw ,
                    //JTokenType.  Null ,
                    //JTokenType.  Undefined ,
                    //              JTokenType.Array ,
                    // JTokenType. Constructor ,
                    //  JTokenType.Property ,
                    //  JTokenType.Comment ,
                    #endregion
                }
                return valueToTypeHash;
            }
        }
        /// <summary>
        /// 字符串连接用字符
        /// </summary>
        private string strJoinSign = FilterConditionConvert.DefaultStrJoinSign;
        /// <summary>
        /// 字符串连接用字符
        /// </summary>
        public string StrJoinSign
        {
            get { return strJoinSign; }
            set { strJoinSign = value; }
        }
        /// <summary>        
        /// 参数名格式，对于Nhibernate为:{0}，对于不同种数据库可能不一样
        /// </summary>
        private string paramerNameFormat = FilterConditionConvert.DefaultParamerNameFormat;
        /// <summary>
        /// 参数名格式，对于Nhibernate为:{0}，对于不同种数据库可能不一样
        /// </summary>
        public string ParamerNameFormat
        {
            get { return paramerNameFormat; }
            set { paramerNameFormat = value; }
        }
        /// <summary>        
        /// 字符型字段表达式格式，主要是要加isnull
        /// </summary>
        private string stringFieldFormat = FilterConditionConvert.DefaultStringFieldFormat;
        /// <summary>
        /// 字符型字段表达式格式，主要是要加isnull
        /// </summary>
        public string StringFieldFormat
        {
            get { return stringFieldFormat; }
            set { stringFieldFormat = value; }
        }
        /// <summary>
        /// 如果，碰到字符串like的话，如何匹配
        /// </summary>
        private string likeFormat = DefaultLikeFormat;
        /// <summary>
        /// 如果，碰到字符串like的话，如何匹配
        /// </summary>
        public string LikeFormat
        {
            get { return likeFormat; }
            set { likeFormat = value; }
        }
        /// <summary>        
        /// 集合参数中参数名格式
        /// </summary>
        private string listSingleParamerNameFormat = DefaultListSingleParamerNameFormat;
        /// <summary>
        /// 集合参数中参数名格式
        /// </summary>
        public string ListSingleParamerNameFormat
        {
            get { return listSingleParamerNameFormat; }
            set { listSingleParamerNameFormat = value; }
        }
        /// <summary>
        /// SQL中的集合参数中参数名格式
        /// </summary>
        private string listSingleParamerNameFormatPart = DefaultListSingleParamerNameFormatPart;
        /// <summary>
        /// SQL中的集合参数中参数名格式
        /// </summary>
        public string ListSingleParamerNameFormatPart
        {
            get { return listSingleParamerNameFormatPart; }
            set { listSingleParamerNameFormatPart = value; }
        }
        #endregion

        #region 获取表达式部分
        /// <summary>
        /// 帮助由FilterCondition获得表达式
        /// </summary>
        /// <param name="conditions">由Ext.Net控件提供</param>
        /// <returns></returns>
        public string GetExpression(IEnumerable<FilterCondition> conditions)
        {
            //传一个空的Hashtable
            SortedList<string, string> gridFileldHashTable = new SortedList<string, string>();
            List<string> expressionList = this.GetExpressionList(conditions, gridFileldHashTable);
            return string.Join(" and ", expressionList.ToArray());
        }
        /// <summary>
        /// 帮助由FilterCondition获得表达式
        /// </summary>
        /// <param name="conditions">由Ext.Net控件提供</param>
        /// <param name="gridPanelFileldHashTable">数据列名和sql列名的对照表</param>
        /// <returns></returns>
        public string GetExpression(IEnumerable<FilterCondition> conditions, IDictionary<string, string> gridPanelFileldHashTable)
        {
            List<string> expressionList = this.GetExpressionList(conditions, gridPanelFileldHashTable);
            return string.Join(" and ", expressionList.ToArray());
        }
        /// <summary>
        /// 帮助由FilterCondition获得表达式
        /// </summary>
        /// <param name="conditions">由Ext.Net控件提供</param>
        /// <param name="gridPanelFileldHashTable">数据列名和sql列名的对照表</param>
        /// <returns></returns>
        public List<string> GetExpressionList(IEnumerable<FilterCondition> conditions, IDictionary<string, string> gridPanelFileldHashTable)
        {
            List<string> expressionList = new List<string>();
            foreach (FilterCondition condition in conditions)
            {
                string expression = string.Empty;
                if (gridPanelFileldHashTable.ContainsKey(condition.Field))
                {
                    expression = this.GetExpression(condition, gridPanelFileldHashTable[condition.Field]);
                }
                else
                {
                    expression = this.GetExpression(condition, condition.Field);
                }
                expressionList.Add(expression);
            }
            return expressionList;
        }
        /// <summary>
        /// 帮助由FilterCondition获得表达式
        /// </summary>
        /// <param name="condition">由Ext.Net控件提供</param>
        /// <param name="sqlFieldName">sql中的列名,带表名.，形式：表名.字段名</param>
        /// <returns></returns>
        public string GetExpression(FilterCondition condition, string sqlFieldName)
        {
            #region 左面部分
            string leftPart = string.Format("{0}", sqlFieldName).Trim();
            if (condition.Type == FilterType.String)
            {
                leftPart = string.Format(StringFieldFormat, sqlFieldName).Trim();
            }
            #endregion

            #region 比较符
            string sign = "=";
            switch (condition.Comparison)
            {
                case Comparison.Eq:
                    {
                        switch (condition.Type)
                        {
                            case FilterType.List:
                                {
                                    sign = "in";
                                    break;
                                }
                            case FilterType.String:
                                {
                                    sign = "like";
                                    break;
                                }
                        }
                        break;
                    }
                case Comparison.Gt:
                    {
                        sign = ">";
                        break;
                    }
                case Comparison.Lt:
                    {
                        sign = "<";
                        break;
                    }
                case Comparison.Like:
                    {
                        sign = "like";
                        break;
                    }
            }
            #endregion

            #region 表达式右面部分
            string rightPart = string.Empty;
            switch (condition.Type)
            {
                case FilterType.Boolean:
                case FilterType.Date:
                case FilterType.Number:
                    {
                        rightPart = this.GetParamerNamePart(condition);
                        break;
                    }
                case FilterType.String:
                    {
                        rightPart = string.Format(LikeFormat, this.GetParamerNamePart(condition));
                        break;
                    }
                case FilterType.List:
                    {
                        List<string> paramerList = this.GetParamerNameListPart(condition);
                        rightPart = string.Join(",", paramerList.ToArray());
                        rightPart = string.Format("({0})", rightPart);
                        break;
                    }
            }
            #endregion

            return string.Format("{0} {1} {2}", leftPart, sign, rightPart);
        }
        #endregion

        #region 获取参数名部分
        /// <summary>
        /// 获取参数名用，通过FilterCondition获得参数名
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected string GetParamerName(FilterCondition condition)
        {
            string field = condition.Field + condition.GetHashCode();
            field = field.Replace("-", "a");
            return field;
        }
        /// <summary>
        /// 语句用，通过FilterCondition获得参数名
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected string GetParamerNamePart(FilterCondition condition)
        {
            return string.Format(this.ParamerNameFormat, this.GetParamerName(condition));
        }
        /// <summary>
        /// 获取参数名用，通过FilterCondition获得参数名集合，适合于condition.List
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected List<string> GetParamerNameList(FilterCondition condition)
        {
            List<string> paramerNameList = new List<string>();
            for (int i = 0; i < condition.List.Count; i++)
            {
                string paramerName = string.Format(this.ListSingleParamerNameFormat, this.GetParamerName(condition), i);
                paramerNameList.Add(paramerName);
            }
            return paramerNameList;
        }
        /// <summary>
        /// 语句用，通过FilterCondition获得参数名集合，适合于condition.List
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected List<string> GetParamerNameListPart(FilterCondition condition)
        {
            List<string> paramerNameList = this.GetParamerNameList(condition);
            List<string> paramerNamePartList = new List<string>();
            for (int i = 0; i < paramerNameList.Count; i++)
            {
                string paramerName = paramerNameList[i];
                string paramerNamePart = string.Format(this.ParamerNameFormat, paramerName);
                paramerNamePartList.Add(paramerNamePart);
            }
            return paramerNamePartList;
        }
        #endregion

        #region 获得参数集合用
        /// <summary>
        /// 从conditionList中提取参数集合
        /// </summary>
        /// <param name="conditionList"></param>
        /// <returns></returns>
        public List<ParamerInfo> GetParamerInfoList(IEnumerable<FilterCondition> conditionList)
        {
            List<ParamerInfo> paramerList = new List<ParamerInfo>();
            foreach (FilterCondition condition in conditionList)
            {
                if (condition.Type != FilterType.List)
                {
                    ParamerInfo paramerInfo = this.GetParamerInfo(condition);
                    paramerList.Add(paramerInfo);
                }
                else
                {
                    List<ParamerInfo> subParamerList = this.GetParamerInfoList(condition);
                    paramerList.AddRange(subParamerList);
                }
            }
            return paramerList;
        }
        /// <summary>
        /// 当condition.FilterType != FilterType.List时，根据condition获得参数集合
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<ParamerInfo> GetParamerInfoList(FilterCondition condition)
        {
            List<ParamerInfo> paramerList = new List<ParamerInfo>();
            for (int i = 0; i < condition.List.Count; i++)
            {
                ParamerInfo paramerInfo = new ParamerInfo();
                paramerInfo.ParamerValue = condition.List[i];
                paramerInfo.ParamerName = string.Format(this.ListSingleParamerNameFormat, this.GetParamerName(condition), i);
                paramerInfo.ParamerType = NHibernate.Type.TypeFactory.GetSerializableType(typeof(string));
                paramerList.Add(paramerInfo);
            }
            return paramerList;
        }
        #endregion

        #region 获得单个参数用
        /// <summary>
        /// 通过condition获得一个参数信息
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public ParamerInfo GetParamerInfo(FilterCondition condition)
        {
            ParamerInfo paramerInfo = new ParamerInfo();
            paramerInfo.ParamerName = this.GetParamerName(condition);
            paramerInfo.ParamerValue = this.GetParamerValue(condition);
            //通过fileld.Type转换成Type typeObj
            Type typeObj = ValueToTypeHash[condition.ValueType];
            //转换成NHibernate用的IType
            paramerInfo.ParamerType = NHibernate.Type.TypeFactory.GetSerializableType(typeObj);
            return paramerInfo;
        }
        #endregion

        #region 获取单个参数值的部分
        /// <summary>
        /// 获取参数名用，通过FilterCondition获得参数名
        /// </summary>
        /// <param name="condition"></param>        
        /// <returns></returns>
        protected object GetParamerValue(FilterCondition condition)
        {
            object value = null;
            switch (condition.ValueType)
            {
                case JTokenType.Boolean:
                    {
                        value = condition.Value<bool>();
                        break;
                    }
                case JTokenType.Date:
                    {
                        value = condition.Value<DateTime>();
                        break;
                    }
                case JTokenType.String:
                    {
                        value = condition.Value<string>();
                        break;
                    }
                case JTokenType.Integer:
                    {
                        value = condition.Value<int>();
                        break;
                    }
                case JTokenType.Float:
                    {
                        value = condition.Value<float>();
                        break;
                    }
            }
            return value;
        }
        #endregion

        #region 获取排序部分
        /// <summary>
        /// 获取单个排序
        /// </summary>
        /// <param name="sorter"></param>
        /// <param name="fieldHashtable"></param>
        /// <returns></returns>
        public string GetSortStr(DataSorter sorter, IDictionary<string, string> fieldHashtable)
        {
            string colName = sorter.Property;
            if (fieldHashtable.ContainsKey(sorter.Property))
            {
                colName = fieldHashtable[sorter.Property];
            }
            string sortStr = string.Format("{0} {1}", colName, sorter.Direction);
            return sortStr;
        }
        /// <summary>
        /// 获取排序集合
        /// </summary>
        /// <param name="sorter"></param>
        /// <param name="fieldHashtable"></param>
        /// <returns></returns>
        public List<string> GetSortList(IEnumerable<DataSorter> sorterList, IDictionary<string, string> fieldHashtable)
        {
            List<string> sortStrList = new List<string>();
            foreach (DataSorter sorter in sorterList)
            {
                sortStrList.Add(GetSortStr(sorter, fieldHashtable));
            }
            return sortStrList;
        }
        /// <summary>
        /// 获取排序集合字符形式
        /// </summary>
        /// <param name="sorter"></param>
        /// <param name="fieldHashtable"></param>
        /// <returns></returns>
        public string GetSortListStr(IEnumerable<DataSorter> sorterList)
        {
            //传一个空的Hashtable
            IDictionary<string, string> fieldHashtable = new SortedList<string, string>();
            List<string> sortStrList = GetSortList(sorterList, fieldHashtable);
            return string.Join(",", sortStrList.ToArray());
        }
        /// <summary>
        /// 获取排序集合字符形式
        /// </summary>
        /// <param name="sorter"></param>
        /// <param name="fieldHashtable"></param>
        /// <returns></returns>
        public string GetSortListStr(IEnumerable<DataSorter> sorterList, IDictionary<string, string> fieldHashtable)
        {
            List<string> sortStrList = GetSortList(sorterList, fieldHashtable);
            return string.Join(",", sortStrList.ToArray());
        }
        #endregion

        /// <summary>
        /// 单例
        /// </summary>
        public static FilterConditionConvert instance = new FilterConditionConvert();
        /// <summary>
        /// 单例
        /// </summary>
        public static FilterConditionConvert Instance
        {
            get { return instance; }
        }
    }
}