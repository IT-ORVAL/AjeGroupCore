using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBM.VCA.Watson.Watson
{

    public class CloudantModel
    {
        public int total_rows { get; set; }
        public int offset { get; set; }
        public Row[] rows { get; set; }
    }

    public class Row
    {
        public string id { get; set; }
        public string key { get; set; }
        public Value value { get; set; }
        public Doc doc { get; set; }
    }

    public class Value
    {
        public string rev { get; set; }
    }

    public class Doc
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string topic { get; set; }
        public Payload payload { get; set; }
        public string deviceId { get; set; }
        public string deviceType { get; set; }
        public string eventType { get; set; }
        public string format { get; set; }
    }

    public class Payload
    {
        public D d { get; set; }
    }

    public class D
    {
        public string Name { get; set; }
        public int temperatura { get; set; }
        public int humedad { get; set; }
    }




    public class CloudantSelector
    {
        public Selector selector { get; set; }
        public string[] fields { get; set; }
        public Sort[] sort { get; set; }
        public int limit { get; set; }
    }

    public class Selector
    {
        public _Id _id { get; set; }
    }

    public class _Id
    {
        public int gt { get; set; }
    }

    public class Sort
    {
        public string _id { get; set; }
    }




    public class CloudantFiltered
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string topic { get; set; }
        public Payload payload { get; set; }
        public string deviceId { get; set; }
        public string deviceType { get; set; }
        public string eventType { get; set; }
        public string format { get; set; }
    }


    public class CloudantFiltered2
    {
        public Doc[] docs { get; set; }
    }


}