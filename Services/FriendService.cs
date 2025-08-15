using Network;
using UnityEngine;
using Services;
using SkillBridge.Message;
using System;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using Models;
using Managers;


namespace Assets.Scripts.Services {
    internal class FriendService : Singleton<FriendService>, IDisposable {

        public UnityAction OnFriendUpdate;

        public void Init() {

        }

        public FriendService() {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddReponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }
        public void Dispose() {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddReponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        internal void SendFriendAddRequest(int friendId, string friendName) {
            Debug.Log("SendFriendAddRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRequest = new FriendAddRequest();
            message.Request.friendAddRequest.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddRequest.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddRequest.ToId = friendId;
            message.Request.friendAddRequest.ToName = friendName;
            NetClient.Instance.SendMessage(message);

        }
        private void SendFriendAddResponse(bool accept, FriendAddRequest request) {
            Debug.Log("OnFriendAddReponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddResponse = new FriendAddResponse();
            message.Request.friendAddResponse.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddResponse.Errormsg = accept ? "对方同意" : "对方拒绝了你的请求";
            message.Request.friendAddResponse.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到好友请求，玩家选择点击接受和拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddRequest(object sender, FriendAddRequest request) {
            var confirm = MessageBox.Show(string.Format("{0} 请求加你好友", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () => {
                this.SendFriendAddResponse(true, request);
            };
            confirm.OnNo = () => {
                this.SendFriendAddResponse(false, request);
            };
        }

        /// <summary>
        /// 收到添加好友响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddReponse(object sender,FriendAddResponse message) {
            if(message.Result == Result.Success) {
                MessageBox.Show(message.Request.ToName + "接受了您的请求", "添加好友成功");
            } else {
                MessageBox.Show(message.Errormsg, "添加好友失败");
            }
        }
        private void OnFriendList(object sender, FriendListResponse message) {
            Debug.Log("OnFriendList");
            FriendManager.Instance.allFriends = message.Friends;
            if(this.OnFriendUpdate != null) {
                this.OnFriendUpdate();
            }
        }
        internal void SendFriendRemoveRequest(int id,int friendId ) {
            Debug.Log("SendFriendRemoveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.friendId = friendId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnFriendRemove(object sender, FriendRemoveResponse message) {
            if(message.Result == Result.Success) {
                MessageBox.Show("删除成功", "删除好友");
            } else {
                MessageBox.Show("删除失败","删除好友",MessageBoxType.Error);
            }
        }


    }
}
