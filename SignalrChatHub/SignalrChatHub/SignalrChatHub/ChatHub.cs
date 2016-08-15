using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNet.SignalR.Hubs;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;


namespace SignalrChatHub
{
    [HubName("ContosoChatHub")]
    public class ChatHub : Hub
    {
        #region Data Members
        private SqlCommand cmd = null;
        private SQLAccess objSql = null;

        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<GroupDetail> GroupUsers = new List<GroupDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();
        //private static readonly List<User> Users = new List<User>();
        //private static readonly List<UserCall> UserCalls = new List<UserCall>();
        //private static readonly List<CallOffer> CallOffers = new List<CallOffer>(); 
        PushSharpNotification.MessageList objMaster = new PushSharpNotification.MessageList();
        #endregion

        //public void Send(string name, string message)
        //{
        //    Clients.All.addMessage(name, message);
        //}
        #region "Single Chat"
        public void sendNotication()
        {
            PushSharpNotification.MessageList objMaster = new PushSharpNotification.MessageList();
            objMaster.sendNotication("", "You have unread message.");
        }
        public void Connect(string conID, string uID)
        {
            //var id = Context.ConnectionId;

            //var UID = Context.QueryString["myInfo"];

            if (ConnectedUsers.Count(x => x.UserID == uID) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = conID, UserID = uID });

                // send to caller
                // Clients.Caller.onConnected(UserID, UserName, ConnectedUsers, CurrentMessage);

                // send to all except caller client
                //Clients.AllExcept(conID).onNewUserConnected(conID, uID);

            }
            else
            {
                var item = ConnectedUsers.FirstOrDefault(x => x.UserID == uID);
                item.ConnectionId = conID;
                ///ConnectedUsers.Add(new UserDetail { ConnectionId = conID, UserID = uID });
            }
            SendPendingMessages(uID);

        }
        public void SendPendingMessages(string uID)
        {
            // Get pending messages for user and send it
            string exMessage = string.Empty;
            objSql = new SQLAccess();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetMessageCount";
            cmd.Parameters.AddWithValue("@UserID", uID);
            cmd.Parameters.AddWithValue("@IsCount", "0");
            DataTable dtResult = objSql.GetDt(cmd, ref exMessage);
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                var meesages = dtResult.AsEnumerable()
                           .Select(r => r.Field<string>("Message"))
                           .ToList();
                var fromUserIDs = dtResult.AsEnumerable()
                               .Select(r => r.Field<int>("USERID"))
                               .ToList();
                var MessageID = dtResult.AsEnumerable()
                               .Select(r => r.Field<int>("MessageID"))
                               .ToList();
                var MesgDateTime = dtResult.AsEnumerable()
                               .Select(r => r.Field<string>("MesgDateTime"))
                               .ToList();
                var MessageType = dtResult.AsEnumerable()
                              .Select(r => r.Field<string>("MessageType"))
                              .ToList();
                Clients.Caller.processPendingMessages(meesages, fromUserIDs, MessageID, MesgDateTime, MessageType);
            }

        }
        public void sendMessage(string fromUserId, string toUserId, string message, string MessageType, string MesgDateTime)
        {

            //string fromUserId = Context.ConnectionId;
            string exMessage = string.Empty;
            //var ConnectionIDList = ConnectedUsers.Single(u => u.UserID == toUserId);
            //var toConnectionID = ConnectionIDList.ConnectionId;
            var toUser = ConnectedUsers.FirstOrDefault(x => x.UserID == toUserId);

            var fromUser = ConnectedUsers.FirstOrDefault(x => x.UserID == fromUserId);
            //var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromConnectionID);
            //var fUserID = fromUser.UserID;
            //Clients.All.addMessage(message);

            objSql = new SQLAccess();
            cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Save_MessageGroupOrPrivate";
            cmd.Parameters.AddWithValue("@FromUserID", fromUserId);
            cmd.Parameters.AddWithValue("@ToUserID", toUserId);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@destination_type", "User");
            cmd.Parameters.AddWithValue("@MessageType", MessageType);
            cmd.Parameters.AddWithValue("@MesgDateTime", MesgDateTime);
            cmd.Parameters.Add("@MessageID", SqlDbType.Int, 4);
            cmd.Parameters["@MessageID"].Direction = ParameterDirection.Output;
            objSql.ExecuteNonQuery(cmd, ref exMessage);
            int MessageID = Convert.ToInt32(cmd.Parameters["@MessageID"].Value);

            if (toUser != null && fromUser != null)
            {
                var toConnectionID = toUser.ConnectionId;
                //// send to 
                //Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message);

                //// send to caller user
                //Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message);

                Clients.Client(toConnectionID).addMessage(fromUserId, toUserId, message, MessageType, MessageID, MesgDateTime);
            }
            else
            {
                objMaster.sendNotication(toUserId, "You have unread message.");
            }
            //else
            //{
            //    objSql = new SQLAccess();
            //    cmd = new SqlCommand();

            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.CommandText = "Save_MessageGroupOrPrivate";
            //    cmd.Parameters.AddWithValue("@FromUserID", fromUserId);
            //    cmd.Parameters.AddWithValue("@ToUserID", toUserId);
            //    cmd.Parameters.AddWithValue("@Message", message);
            //    cmd.Parameters.AddWithValue("@destination_type", "User");
            //    cmd.Parameters.AddWithValue("@MessageType", MessageType);
            //    cmd.Parameters.Add("@MessageID", SqlDbType.Int, 4);
            //    cmd.Parameters["@MessageID"].Direction = ParameterDirection.Output;
            //    objSql.ExecuteNonQuery(cmd, ref exMessage);
            //    int MessageID = Convert.ToInt32(cmd.Parameters["@MessageID"].Value);
            //}

        }
        public void GetUnreadMessageCount(string UserID)
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSql = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            string strFileName = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMessageCount";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@IsCount", "1");
                dtResult = objSql.GetDt(cmd, ref exMessage);
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "MessageCountDetail";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            var UserList = ConnectedUsers.FirstOrDefault(x => x.UserID == UserID);
            if (UserList != null)
            {
                var ConnectionID = UserList.ConnectionId;
                Clients.Client(ConnectionID).recievedMessageCount(JsonConvert.SerializeObject(dsDataSet, Formatting.None));
            }
           
        }
        public string RemoveMessageFromServer(string UserID)
        {
            objSql = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RemoveMessageFromServer";
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@MessageID", 0);
            cmd.Parameters.AddWithValue("@IsMessageID", 0);
            int i = objSql.ExecuteNonQuery(cmd, ref exMessage);
            //Clients.Client(ConnectionID).recievedMessageCount();
            if (exMessage == string.Empty)
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }
        public string RemoveMessageFromServerUsingMessageID(string MessageID)
        {
            objSql = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "RemoveMessageFromServer";
            cmd.Parameters.AddWithValue("@UserID", 0);
            cmd.Parameters.AddWithValue("@MessageID", MessageID);
            cmd.Parameters.AddWithValue("@IsMessageID", 1);
            int i = objSql.ExecuteNonQuery(cmd, ref exMessage);
            //Clients.Client(ConnectionID).recievedMessageCount();
            if (exMessage == string.Empty)
            {
                return "True";
            }
            else
            {
                return "False";
            }

        }
        #endregion
        #region "Group Chat"
        public void Get_Connect(String userid, String connectionid, String GroupID)
        {
            if (GroupUsers.Count(x => x.UserID == userid) == 0)
            {
                GroupUsers.Add(new GroupDetail { ConnectionId = connectionid, UserID = userid, GroupID = GroupID });
                Groups.Add(connectionid, GroupID);
            }

        }
        public void Get_Remove(String userid, String connectionid, String GroupID)
        {
            if (GroupUsers.Count(x => x.UserID == userid) >= 0)
            {
                var item = GroupUsers.FirstOrDefault(x => x.UserID == userid);
                GroupUsers.Remove(item);
                Groups.Remove(connectionid, GroupID);
            }
        }
        public void sendMessageToGroup(string fromID, string groupID, string message, string dateTime, string MessageType, string connectionID,string strUserName)
        {
            // Call the addMessage method on all clients            
            Clients.Group(groupID, connectionID).receiveGroupMessage(fromID, groupID, message, dateTime, MessageType,strUserName);

            string strListUser = string.Empty;
            foreach (var item in ConnectedUsers)
            {
                strListUser = strListUser + item.UserID + ",";
            }
            if (strListUser != null)
            {
                strListUser = strListUser.Remove(strListUser.Length - 1);
                string exMessage = string.Empty;
                objSql = new SQLAccess();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddGroupOfflineUserMesseage";
                cmd.Parameters.AddWithValue("@UserID", strListUser);
                cmd.Parameters.AddWithValue("@GroupID", groupID);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@FromUserID", fromID);
                cmd.Parameters.AddWithValue("@MessageType", MessageType);
                int i = objSql.ExecuteNonQuery(cmd, ref exMessage);
            }
            //string replaced = "'" + str.Replace(",", "','") + "'";

        }
        #endregion

        public void SendCall(string fromUserId, string toUserId, byte[] bytearry)
        {
            var toUser = ConnectedUsers.FirstOrDefault(x => x.UserID == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.UserID == fromUserId);
            if (toUser != null && fromUser != null)
            {
                var toConnectionID = toUser.ConnectionId;
                Clients.Client(toConnectionID).RecivedCall(fromUserId, toUserId,bytearry);
            }
        }
        public void AudioCall(string fromUserId, string toUserId, string buffer, string bufferSize)
        {
            var toUser = ConnectedUsers.FirstOrDefault(x => x.UserID == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.UserID == fromUserId);
            if (toUser != null && fromUser != null)
            {
                var toConnectionID = toUser.ConnectionId;
                Clients.Client(toConnectionID).RecivedAudio(fromUserId,toUserId,buffer,bufferSize);
            }
        }
        public void SendCallJson(string fromUserId, string toUserId, string stringBytes)
        {
            var toUser = ConnectedUsers.FirstOrDefault(x => x.UserID == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.UserID == fromUserId);
            if (toUser != null && fromUser != null)
            {
                var toConnectionID = toUser.ConnectionId;
                Clients.Client(toConnectionID).RecivedCall(fromUserId, toUserId, stringBytes);
            }
        }
        public void SendImage(string fromUserId, string toUserId, string strUserImage)
        {
            string strFileName = string.Empty;
            //string fromUserId = Context.ConnectionId;
            string exMessage = string.Empty;
            //var ConnectionIDList = ConnectedUsers.Single(u => u.UserID == toUserId);
            //var toConnectionID = ConnectionIDList.ConnectionId;
            var toUser = ConnectedUsers.FirstOrDefault(x => x.UserID == toUserId);

            var fromUser = ConnectedUsers.FirstOrDefault(x => x.UserID == fromUserId);
            //var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromConnectionID);
            //var fUserID = fromUser.UserID;



            if (toUser != null && fromUser != null)
            {
                var toConnectionID = toUser.ConnectionId;
                //// send to 
                //Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message);

                //// send to caller user
                //Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message);
                Clients.Client(toConnectionID).addImage(strFileName);

            }
            else
            {
                objSql = new SQLAccess();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Save_MessageGroupOrPrivate";
                cmd.Parameters.AddWithValue("@FromUserID", fromUserId);
                cmd.Parameters.AddWithValue("@ToUserID", toUserId);
                cmd.Parameters.AddWithValue("@Message", strFileName);
                cmd.Parameters.AddWithValue("@destination_type", "User");
                cmd.Parameters.AddWithValue("@status", "1");
                objSql.ExecuteNonQuery(cmd, ref exMessage);
            }

        }
        
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (item != null)
                {
                    ConnectedUsers.Remove(item);

                    var id = Context.ConnectionId;
                    Clients.All.onUserDisconnected(id, item.UserID);

                }
            }
            else
            {
                Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
            }

            return base.OnDisconnected(stopCalled);
        }

        public void RemoveFromServerUser(string UserID)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.UserID == UserID);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = item.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserID);
            }

             base.OnDisconnected(true);
        }

        //#region "Group Chat"
        //public void CallUser(string fromUserId, string toUserId)
        //{
        //    var callingUser = Users.SingleOrDefault(u => u.ConnectionId == fromUserId);
        //    var targetUser = Users.SingleOrDefault(u => u.ConnectionId == toUserId);

        //    // Make sure the person we are trying to call is still here
        //    if (targetUser == null)
        //    {
        //        // If not, let the caller know
        //        Clients.Caller.callDeclined(targetUser, "The user you called has left.");
        //        return;
        //    }

        //    // And that they aren't already in a call
        //    if (GetUserCall(targetUser.ConnectionId) != null)
        //    {
        //        Clients.Caller.callDeclined(targetUser, string.Format("{0} is already in a call."));
        //        return;
        //    }

        //    // They are here, so tell them someone wants to talk
        //    Clients.Client(targetUser.ConnectionId).incomingCall(callingUser);

        //    // Create an offer
        //    CallOffers.Add(new CallOffer
        //    {
        //        Caller = callingUser,
        //        Callee = targetUser
        //    });
        //}
        //public void Join(string username)
        //{
        //    // Add the new user
        //    Users.Add(new User
        //    {
        //        Username = username,
        //        ConnectionId = Context.ConnectionId
        //    });

        //    // Send down the new list to all clients
        //    SendUserListUpdate();
        //}

        //public void AnswerCall(bool acceptCall, string targetConnectionId)
        //{
        //    var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        //    var targetUser = Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

        //    // This can only happen if the server-side came down and clients were cleared, while the user
        //    // still held their browser session.
        //    if (callingUser == null)
        //    {
        //        return;
        //    }

        //    // Make sure the original caller has not left the page yet
        //    if (targetUser == null)
        //    {
        //        Clients.Caller.callEnded(targetConnectionId, "The other user in your call has left.");
        //        return;
        //    }

        //    // Send a decline message if the callee said no
        //    if (acceptCall == false)
        //    {
        //        Clients.Client(targetConnectionId).callDeclined(callingUser, string.Format("{0} did not accept your call.", callingUser.Username));
        //        return;
        //    }

        //    // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
        //    var offerCount = CallOffers.RemoveAll(c => c.Callee.ConnectionId == callingUser.ConnectionId
        //                                          && c.Caller.ConnectionId == targetUser.ConnectionId);
        //    if (offerCount < 1)
        //    {
        //        Clients.Caller.callEnded(targetConnectionId, string.Format("{0} has already hung up.", targetUser.Username));
        //        return;
        //    }

        //    // And finally... make sure the user hasn't accepted another call already
        //    if (GetUserCall(targetUser.ConnectionId) != null)
        //    {
        //        // And that they aren't already in a call
        //        Clients.Caller.callDeclined(targetConnectionId, string.Format("{0} chose to accept someone elses call instead of yours :(", targetUser.Username));
        //        return;
        //    }

        //    // Remove all the other offers for the call initiator, in case they have multiple calls out
        //    CallOffers.RemoveAll(c => c.Caller.ConnectionId == targetUser.ConnectionId);

        //    // Create a new call to match these folks up
        //    UserCalls.Add(new UserCall
        //    {
        //        Users = new List<User> { callingUser, targetUser }
        //    });

        //    // Tell the original caller that the call was accepted
        //    Clients.Client(targetConnectionId).callAccepted(callingUser);

        //    // Update the user list, since thes two are now in a call
        //    SendUserListUpdate();
        //}

        //public void HangUp()
        //{
        //    var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

        //    if (callingUser == null)
        //    {
        //        return;
        //    }

        //    var currentCall = GetUserCall(callingUser.ConnectionId);

        //    // Send a hang up message to each user in the call, if there is one
        //    if (currentCall != null)
        //    {
        //        foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
        //        {
        //            Clients.Client(user.ConnectionId).callEnded(callingUser.ConnectionId, string.Format("{0} has hung up.", callingUser.Username));
        //        }

        //        // Remove the call from the list if there is only one (or none) person left.  This should
        //        // always trigger now, but will be useful when we implement conferencing.
        //        currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
        //        if (currentCall.Users.Count < 2)
        //        {
        //            UserCalls.Remove(currentCall);
        //        }
        //    }

        //    // Remove all offers initiating from the caller
        //    CallOffers.RemoveAll(c => c.Caller.ConnectionId == callingUser.ConnectionId);

        //    SendUserListUpdate();
        //}
        //public void SendSignal(string signal, string targetConnectionId)
        //{
        //    var callingUser = Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
        //    var targetUser = Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

        //    // Make sure both users are valid
        //    if (callingUser == null || targetUser == null)
        //    {
        //        return;
        //    }

        //    // Make sure that the person sending the signal is in a call
        //    var userCall = GetUserCall(callingUser.ConnectionId);

        //    // ...and that the target is the one they are in a call with
        //    if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
        //    {
        //        // These folks are in a call together, let's let em talk WebRTC
        //        Clients.Client(targetConnectionId).receiveSignal(callingUser, signal);
        //    }
        //}
        //#endregion
        //#region Private Helpers

        //private void SendUserListUpdate()
        //{
        //    Users.ForEach(u => u.InCall = (GetUserCall(u.ConnectionId) != null));
        //    Clients.All.updateUserList(ConnectedUsers);
        //}

        //private UserCall GetUserCall(string connectionId)
        //{
        //    var matchingCall =
        //        UserCalls.SingleOrDefault(uc => uc.Users.SingleOrDefault(u => u.ConnectionId == connectionId) != null);
        //    return matchingCall;
        //}

        //#endregion
    }
}