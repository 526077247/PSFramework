using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class DataList<T> : List<T>
    {
        public DataList():base()
        {
        
        }

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
                Clear();
                if (value != null)
                {
                    var jObject = JsonConvert.DeserializeObject<List<T>>(value);
                    AddRange(jObject);
                }
            }
        }
    }
}
