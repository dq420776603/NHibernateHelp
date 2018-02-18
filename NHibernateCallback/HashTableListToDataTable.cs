using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace NHibernateHelp.NHibernateCallback
{
    public class HashTableListToDataTable
    {
        public static DataTable ToDataTable(IEnumerable infoList)
        {
            List<IDictionary> infoList1 = new List<IDictionary>();
            foreach (var info in infoList)
            {
                infoList1.Add(info as IDictionary);
            }
            return ToDataTable(infoList1);
        }
        public static DataTable ToDataTable(IEnumerable<IDictionary> infoList)
        {
            DataTable dt = new DataTable();
            if (infoList != null)
            {
                //为dt加列，如果，某行某列为null,则当前列到下一行去找列信息
                foreach (IDictionary info in infoList)
                {
                    //循环加列
                    foreach (DictionaryEntry pro in info)
                    {
                        if (pro.Value != null && !dt.Columns.Contains(string.Format("{0}", pro.Key)))
                        {
                            DataColumn col = dt.Columns.Add(string.Format("{0}", pro.Key));
                            col.DataType = pro.Value.GetType();
                        }
                    }
                    //找是否有上文没加上的列，那说明那列的Value是null
                    bool isHaveNullCol = false;
                    foreach (DictionaryEntry pro in info)
                    {
                        //如果，没有当前列，则isHaveNullCol = true;跳出当前循环，外层循环正好进入下次循环
                        if (!dt.Columns.Contains(string.Format("{0}", pro.Key)))
                        {
                            isHaveNullCol = true;
                            break;
                        }
                    }
                    //如果没有空列，就跳出循环
                    if (!isHaveNullCol)
                    {
                        break;
                    }
                }
                //循环将Hashtable集合转换为，如果，上文过后，还有列没加上的话，则在此循环中将其补上，DataType不设置
                bool isFirst = true; //是否是第一次进入循环
                foreach (IDictionary info in infoList)
                {
                    //如果是第一次循环，则检查
                    if (isFirst)
                    {
                        foreach (DictionaryEntry pro in info)
                        {
                            //如果，没有当前列，则isHaveNullCol = true;跳出当前循环，外层循环正好进入下次循环
                            if (!dt.Columns.Contains(string.Format("{0}", pro.Key)))
                            {
                                DataColumn col = dt.Columns.Add(string.Format("{0}", pro.Key));
                                if (pro.Value != null)
                                {
                                    col.DataType = pro.Value.GetType();
                                }
                            }
                        }
                    }
                    DataRow dataRow = dt.NewRow();
                    foreach (DictionaryEntry pro in info)
                    {
                        dataRow[string.Format("{0}", pro.Key)] = pro.Value ?? DBNull.Value;
                    }
                    dt.Rows.Add(dataRow);
                    isFirst = false;
                }
            }
            return dt;
        }
    }
}
