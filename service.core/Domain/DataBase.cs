using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace service.core
{
    public class DataBase
    {
        /// <summary>
        /// json序列化与反序列化
        /// </summary>
        [JsonIgnore]
        public string JsonText
        {
            get
            {
                return JsonConvert.SerializeObject(this);
            }
            set
            {
                var jObject = JsonConvert.DeserializeObject<Hashtable>(value);
                foreach (var item in GetType().GetRuntimeProperties())
                    if (item.Name != "JsonText" && jObject[item.Name] != null)
                    {
                        object newvalue;
                        try
                        {
                            newvalue = Convert.ChangeType(jObject[item.Name], item.PropertyType);
                            item.SetValue(this, newvalue);
                        }
                        catch
                        {
                            newvalue = JsonConvert.DeserializeObject(jObject[item.Name].ToString(), item.PropertyType);
                            item.SetValue(this, newvalue);
                        }
                    }
            }
        }
    }
}
