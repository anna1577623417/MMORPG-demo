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
    internal class TeamService : Singleton<TeamService>, IDisposable {


        public void Init() {

        }

        public TeamService() {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);

        }

        public void Dispose() {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

      

        internal void SendInviteRequest(int friendId, string friendName) {
            Debug.Log("SendInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteRequest = new TeamInviteRequest();
            message.Request.teamInviteRequest.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteRequest.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteRequest.ToId = friendId;
            message.Request.teamInviteRequest.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendTeamInviteResponse(bool accept,TeamInviteRequest request) {
            Debug.Log("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteResponse = new TeamInviteResponse();
            message.Request.teamInviteResponse.Result =accept ? Result.Success:Result.Failed;
            message.Request.teamInviteResponse.Errormsg= accept ?"组队成功" : "对方拒绝了组队请求";
            message.Request.teamInviteResponse.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到进入别人队伍 的请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteRequest(object sender, TeamInviteRequest request) {
            var confirm = MessageBox.Show(string.Format(" {0} 邀请你加入队伍", request.FromName), "组队请求",MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () => {
                this.SendTeamInviteResponse(true, request);
            };
            confirm.OnNo = () => {
                this.SendTeamInviteResponse(false, request);
            };
        }

        /// <summary>
        /// 收到组队要求的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteResponse(object sender, TeamInviteResponse message) {
            if(message.Result == Result.Success) {
                MessageBox.Show(message.Request.ToName + "加入您的队伍", "邀请组队成功");
            } else {
                MessageBox.Show(message.Errormsg, "邀请组队失败");
            }
        }

        private void OnTeamInfo(object sender, TeamInfoResponse message) {
            Debug.Log("OnTeamInfo");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }
        internal void SendTeamLeaveRequest(int id) {
            Debug.Log("SendTeamLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new teamLeaveRequest();
            message.Request.teamLeave.TeamId = User.Instance.TeamInfo.Id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }

        private void OnTeamLeave(object sender, TeamLeaveResponse message) {
            if(message.Result == Result.Success) {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("退出成功", "退出队伍");
            } else {
                MessageBox.Show("退出失败","退出队伍",MessageBoxType.Error);
            }
        }

    }
}
