using System;
using System.Collections.Generic;
using System.Text;

namespace service.core
{
    public class ResultList<T>
    {
        public List<T> list { get; set; }
        public int total { get; set; }
        public int pageSize { get; set; }
        public int start { get; set; }
    }
}
