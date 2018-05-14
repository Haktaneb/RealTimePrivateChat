using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeChatRoom.Common
{
    public class UserContent
    {
        public String ConnectionIp { get; set; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string UserState { get; set; }
        public string userImage { get; set; }
    }
}