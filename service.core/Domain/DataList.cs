using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace service.core
{
    class Packet:DataObject
    {
        public object DataPacket { get; set; }
    }
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
        /// <summary>
        /// XML序列化与反序列化
        /// </summary>
        [JsonIgnore]
        public string XMLText
        {

            get
            {
                Packet packet = new Packet();
                packet.DataPacket = this;
                return JsonConvert.DeserializeXNode(packet.JsonText, "DataList", true).ToString();
            }
            set
            {
                XNode doc = XElement.Parse(value);
                var newvalue = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeXNode(doc));
                JsonText = newvalue.First.Value<JToken>().First.Value<JToken>().First.Value<JToken>().First.ToString();
            }
        }
    }
}
