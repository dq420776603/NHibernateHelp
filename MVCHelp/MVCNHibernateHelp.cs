using DBHelp.SqlScriptDom;
using Extend.MVCHelp;
using Extend.SqlScriptDom.SqlPartN;
using NHibernateHelp.NHibernateCallback;
using Spring.Data.NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NHibernateHelp.MVCHelp
{
    public class MVCNHibernateHelp
    {
        #region 转换sql部分
        /// <summary>
        /// 将sql+request表达的部分，重新构建sql，考虑NHibernate参数为:的情况
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlPart RefactorToSqlPart(MVCRequest request, string sql)
        {
            string sqlCopy = sql.Replace(":", "@");
            SqlPart sqlPart = MVCRequestHelp.RefactorToSqlPart(request, sqlCopy);
            sqlPart = ConvertSqlPart(sqlPart);
            return sqlPart;
        }
        /// <summary>
        /// 将sql+request表达的部分，重新构建sql，考虑NHibernate参数为:的情况
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlPart RefactorToSqlPart(string sql)
        {
            string sqlCopy = sql.Replace(":", "@");
            SqlPart sqlPart = MVCRequestHelp.RefactorToSqlPart(sqlCopy);
            sqlPart = ConvertSqlPart(sqlPart);
            return sqlPart;
        }
        /// <summary>
        /// 将sqlPart中各部分:代表的参数名替换成@
        /// </summary>
        /// <param name="sqlPart"></param>
        /// <returns></returns>
        public static SqlPart ConvertSqlPart(SqlPart sqlPart)
        {
            SqlPart sqlPart1 = new SqlPart()
            {
                FieldsSqlPart = sqlPart.FieldsSqlPart.Replace("@", ":"), //参数符号替回来，如果传进来的是 @，正好替成:
                FromSqlPart = sqlPart.FromSqlPart.Replace("@", ":"),
                WhereSqlPart = sqlPart.WhereSqlPart.Replace("@", ":"),
                OrderSqlPart = sqlPart.OrderSqlPart.Replace("@", ":")
            };
            return sqlPart1;
        }
        #endregion

        #region Nhibernate相关
        /// <summary>
        /// Build一个分页用的Pager
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ExecSqlQuery BuildPager(MVCRequest request)
        {
            SqlPager queryEr = new SqlPager();
            queryEr.FirstResult = request.Offset;
            queryEr.MaxResults = request.Rows;
            return queryEr;
        }
        /// <summary>
        /// Build一个分页用的Pager
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <param name="queryEr">默认是一个分页的SqlPager类型</param>
        /// <returns></returns>
        public static ExecSqlQuery BuildPager(MVCRequest request, object args, string sql = null)
        {
            ExecSqlQuery queryEr = BuildPager(request);
            return BuildSqlQuery(args, sql, queryEr);
        }
        /// <summary>
        /// Build一个分页用的Pager
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <param name="queryEr">默认是一个分页的SqlPager类型</param>
        /// <returns></returns>
        public static ExecSqlQuery BuildPager(MVCRequest request, IDictionary<string, object> args, string sql = null)
        {
            ExecSqlQuery queryEr = BuildPager(request);
            return BuildSqlQuery(args, sql, queryEr);
        }
        /// <summary>
        /// 如果需要不分页的数据，调用这个函数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static ExecSqlQuery BuildSqlQuery(IDictionary<string, object> args, string sql = null)
        {
            ExecSqlQuery queryEr = new ExecSqlQuery();
            return BuildSqlQuery(args, sql, queryEr);
        }
        /// <summary>
        /// 如果需要不分页的数据，调用这个函数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static ExecSqlQuery BuildSqlQuery(object args, string sql = null)
        {
            ExecSqlQuery queryEr = new ExecSqlQuery();
            return BuildSqlQuery(args, sql, queryEr);
        }
        /// <summary>
        /// 将request、args、sql所代表的属性赋值给queryEr
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <param name="queryEr">有可能是ExecSqlQuery或SqlPager类型</param>
        /// <returns></returns>
        public static ExecSqlQuery BuildSqlQuery(IDictionary<string, object> args, string sql, ExecSqlQuery queryEr)
        {
            queryEr.ParamerList.AddRange(MVCNHibernateHelp.ConvertToParamerList(args, sql));
            return queryEr;
        }
        /// <summary>
        /// 将request、args、sql所代表的属性赋值给queryEr
        /// </summary>
        /// <param name="request"></param>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <param name="queryEr">有可能是ExecSqlQuery或SqlPager类型</param>
        /// <returns></returns>
        public static ExecSqlQuery BuildSqlQuery(object args, string sql, ExecSqlQuery queryEr)
        {
            queryEr.ParamerList.AddRange(MVCNHibernateHelp.ConvertToParamerList(args, sql));
            return queryEr;
        }
        #endregion

        #region EasyUI返回的查询参数转换为NHibernate需要的参数
        /// <summary>
        /// EasyUI返回的查询参数转换为NHibernate需要的参数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<ParamerInfo> ConvertToParamerList(object args, string sql = null)
        {
            sql = sql ?? string.Empty;
            sql = sql.ToLower();
            sql = sql.Replace("@", ":");
            List<ParamerInfo> paramerInfoList = new List<ParamerInfo>();
            if (args != null)
            {
                //if (!(args is IDictionary))
                //{
                Type type = args.GetType();
                PropertyInfo[] propertyList = type.GetProperties();
                foreach (PropertyInfo property in propertyList)
                {
                    Type paramerType = property.PropertyType;
                    //如果，是泛型并且是值类型（就是说是空属类型）的话
                    if (paramerType.IsGenericType
                        && paramerType.IsSubclassOf(typeof(System.ValueType)))
                    {
                        Type[] typeList = property.PropertyType.GetGenericArguments();
                        if (typeList != null && typeList.Length > 0)
                        {
                            paramerType = typeList[0];
                        }
                    }
                    string paramerName = property.Name.ToLower();
                    string sqlParamerName = ":" + paramerName;
                    #region 本想弄全字匹配，但不好用，用正则来查找全字匹配
                    //string pattern = String.Format(@"\b{0}\b", sqlParamerName);
                    //Regex.IsMatch(sql, pattern, RegexOptions.IgnoreCase);
                    #endregion
                    //如果sql为空则为不检查，或者sql中存在这个参数
                    if (string.IsNullOrWhiteSpace(sql) || sql.Contains(sqlParamerName))
                    {
                        paramerInfoList.Add(new ParamerInfo()
                        {
                            ParamerName = property.Name,
                            ParamerValue = property.GetValue(args, new object[] { }),
                            ParamerType = NHibernate.Type.TypeFactory.GetSerializableType(paramerType)
                        });
                    }
                }
                //}
            }
            return paramerInfoList;
        }
        /// <summary>
        /// EasyUI返回的查询参数转换为NHibernate需要的参数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<ParamerInfo> ConvertToParamerList(IDictionary<string, object> args, string sql = null)
        {
            sql = sql ?? string.Empty;
            sql = sql.ToLower();
            sql = sql.Replace("@", ":");
            List<ParamerInfo> paramerInfoList = new List<ParamerInfo>();
            if (args != null)
            {
                foreach (KeyValuePair<string, object> property in args)
                {
                    string paramerName = property.Key.ToLower();
                    string sqlParamerName = ":" + paramerName;
                    //如果sql为空则为不检查，或者sql中存在这个参数
                    if (string.IsNullOrWhiteSpace(sql) || sql.Contains(sqlParamerName))
                    {
                        paramerInfoList.Add(new ParamerInfo()
                        {
                            ParamerName = property.Key,
                            ParamerValue = property.Value ?? string.Empty,
                            ParamerType = property.Value == null ? NHibernate.Type.TypeFactory.GetSerializableType(typeof(string))
                                    : NHibernate.Type.TypeFactory.GetSerializableType(property.Value.GetType())
                        });
                    }
                }
            }
            return paramerInfoList;
        }
        #endregion
    }
}
