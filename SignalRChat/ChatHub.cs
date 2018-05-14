using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using RealTimeChatRoom.Common;

namespace RealTimeChatRoom
{
    public class ChatHub : Hub
    {

        static List<MessageContent> MessageList = new List<MessageContent>();
        static List<UserContent> UserList = new List<UserContent>();
        
        static string yourIp;




        public String GetIp()
        {
            var serverVars = Context.Request.GetHttpContext().Request.ServerVariables;
            var Ip = serverVars["REMOTE_HOST"];
            return Ip;

        }
        public void GetIp2()
        {

            var serverVars = Context.Request.GetHttpContext().Request.ServerVariables;
            var Ip = serverVars["REMOTE_HOST"];
            yourIp = Ip;
            Clients.Caller.showIp(yourIp);

        }

        public void Connect(string userName,string image)
        {
            var id = Context.ConnectionId;
            var ip = GetIp();

            var state = "Online";

            if (UserList.Count(x => x.ConnectionId == id) == 0)
            {
                UserList.Add(new UserContent { ConnectionIp = ip, ConnectionId = id, UserName = userName, UserState = state,userImage = image });

                // send to caller
                Clients.Caller.userConnection(id, userName, UserList, MessageList, ip, state);

                // send to all except caller client
                Clients.AllExcept(id).NewUserConnected(id, userName, ip, state,image);

            }

        }

        public void SendMessage(string message)
        {

            string userWhoSendID = Context.ConnectionId;
            var senderUser = UserList.FirstOrDefault(x => x.ConnectionId == userWhoSendID);

            

            // display messages
            Clients.All.messageCatcher(senderUser.UserName, message);
        }

        public void SendPrivateMessage(string userToSendID, string message)
        {

            string userWhoSendID = Context.ConnectionId;

            // Receiver
            var ReceiverUser = UserList.FirstOrDefault(x => x.ConnectionId == userToSendID);

            //Sender
            var senderUser = UserList.FirstOrDefault(x => x.ConnectionId == userWhoSendID);

            if (ReceiverUser != null && senderUser != null)
            {

                // send to  by id
                Clients.Client(userToSendID).sendPrivateMessage(userWhoSendID, senderUser.UserName, message);

                // send to caller user
                Clients.Caller.sendPrivateMessage(userToSendID, senderUser.UserName, message);
            }

        }
        //Disconnect
        public void DisconnectUser(string userWhoSendID)
        {
            var item = UserList.FirstOrDefault(x => x.ConnectionId == userWhoSendID);

            if (item != null)
            {
                item.UserState = "Offline";

               
                Clients.All.userDisconnect( UserList);

            }
        }
        //Reconnect
        public void ReconnectUser(string userWhoSendID)
        {
            var item = UserList.FirstOrDefault(x => x.ConnectionId == userWhoSendID);

            if (item != null)
            {
                item.UserState = "Online";

                var id = Context.ConnectionId;
                Clients.All.userReconnect(UserList);

            }
        }
        //Online Ofline Control
        public void StateControl(string userWhoSendID)
        {
            var item = UserList.FirstOrDefault(x => x.ConnectionId == userWhoSendID);

            if (item != null)
            {
               

                var id = Context.ConnectionId;
                Clients.Caller.userStateControl(id, item.UserState);

            }
        }


        //Log out
        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var item = UserList.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                UserList.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);

            }

            return base.OnDisconnected();
        }







    }

}