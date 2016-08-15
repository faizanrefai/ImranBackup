using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApplication1.CommonClass
{
    public class clsCommon
    {

        
        
        public class CombineList
        {
            public IList<BaseClass.FirstList> FirstListItem { get; set; }
            public IList<BaseClass.SecondList> SecondListItem { get; set; }
        }

        public class ListsofObject
        {
            public CombineList AllListCombine { get; set; }
        }

        public class Header
        {
            public string Message { get; set; }
        }
        public class Response
        {
            public string Status { get; set; }
        }
        public class Error
        {
            public string Message { get; set; }
        }

        public class MainDataset
        {
            public ListsofObject ListsofObject { get; set; }
            public Response Response { get; set; }
            public Header Header { get; set; }
            public Error Error { get; set; }
        }
    }
}