using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
                var jObject = JsonConvert.DeserializeObject(value, GetType());
                foreach (var item in GetType().GetRuntimeProperties())
                {
                    if (!item.HasAttribute<JsonIgnoreAttribute>())
                    {
                        var itemValue = item.GetValue(jObject);
                        if (itemValue != null)
                            item.SetValue(this, itemValue);
                    }
                }
            }
        }
        /// <summary>
        /// XML序列化与反序列化
        /// </summary>
        [JsonIgnore]
        public string XMLText
        {

            get
            {
                return JsonConvert.DeserializeXNode(JsonText, "DataPacket", true).ToString();
            }
            set
            {
                XNode doc = XElement.Parse(value);
                var newvalue = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeXNode(doc));
                JsonText = newvalue.First.Value<JToken>().First.ToString();
            }
        }
    }
}
