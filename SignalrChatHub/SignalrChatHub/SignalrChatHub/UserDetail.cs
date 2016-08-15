using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalrChatHub
{
    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserID { get; set; }
        //public string UserName { get; set; }
    }
    public class GroupDetail
    {
        public string ConnectionId { get; set; }
        public string UserID { get; set; }
        public string GroupID { get; set; }
    }
}
