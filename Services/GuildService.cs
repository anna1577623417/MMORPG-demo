using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Managers;

namespace Assets.Scripts.Services {
    internal class GuildService : Singleton<GuildService>,IDisposable {
        public UnityAction OnGuildUpdate;
        public UnityAction<bool> OnGuildCreateResult;

        public UnityAction<List<NGuildInfo>> OnGuildListResult;

        public void Init() {

        }

        public GuildService() {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnguildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmini);
        }


        public void Dispose() {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnguildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

        /// <summary>
        /// 发送创建公会
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="notice"></param>
        internal void SendGuildCreate(string guildName, string notice) {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到公会创建响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnguildCreate(object sender, GuildCreateResponse response) {
            Debug.LogFormat("OnGuildCreateResponse：{0}",response.Result);
            if(OnGuildCreateResult != null) {
                this.OnGuildCreateResult(response.Result ==Result.Success);
            }
            if(response.Result == Result.Success) {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0} 公会创建成功", response.guildInfo.GuildName), "公会");
            } else {
                MessageBox.Show(string.Format("{0} 公会创建失败", response.guildInfo.GuildName), "公会");
            }
        }

        /// <summary>
        /// 发送加入公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void SendGuildJoinResponse(bool accept, GuildJoinRequest request) {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinResponse = new GuildJoinResponse();
            message.Request.guildJoinResponse.Result = Result.Success;
            message.Request.guildJoinResponse.Apply = request.Apply;
            message.Request.guildJoinResponse.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinRequest(int guildId) {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRequest = new GuildJoinRequest();
            message.Request.guildJoinRequest.Apply = new NGuildApplyInfo();
            message.Request.guildJoinRequest.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到加入公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request) {
            Debug.Log("OnGuildJoinRequest");
            var confirm = MessageBox.Show(string.Format("{0} 申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm, "接受", "拒绝");

            confirm.OnYes = () => {
                this.SendGuildJoinResponse(true, request);
            };

            confirm.OnNo = () => {
                this.SendGuildJoinResponse(false, request);
            };
        }

        /// <summary>
        /// 收到加入公会的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response) {
            Debug.LogFormat("OnGuildJoinResponse：{0}",response.Result);
            if(response.Result == Result.Success) {
                MessageBox.Show("加入公会成功", "公会");
            } else {
                MessageBox.Show("加入公会失败", "公会");
            }
        }

        private void OnGuild(object sender, GuildResponse message) {
            Debug.LogFormat("OnGuild：{0} {1} {2}",message.Result,message.guildInfo.Id,message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if(this.OnGuildUpdate != null) {
                this.OnGuildUpdate();
            }
        }

        public void SendGuildLeaveRequest() {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);

        }

        public void OnGuildLeave(object sender,GuildLeaveResponse message) {
            if(message.Result == Result.Success) {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            } else {
                MessageBox.Show("离开公会失败","公会",MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 发送公会列表刷新请求
        /// </summary>
        internal void SendGuildListRequest() {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 当公会列表刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildList(object sender, GuildListResponse response) {
            if(OnGuildListResult != null) {
                if (OnGuildListResult != null) {
                    this.OnGuildListResult(response.Guilds);
                }
            }
        }

        /// <summary>
        /// 发送管理指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="id"></param>
        internal void SendAdminiCommand(GuildAdminCommand cmd, int characterId) {
            Debug.Log("SendAdminiCommand");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = cmd;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);

        }

        /// <summary>
        /// 发送加入公会审批
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept,NGuildApplyInfo apply) {
            Debug.Log("SendGuildJoinApply");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinResponse = new GuildJoinResponse();
            message.Request.guildJoinResponse.Result = Result.Success;
            message.Request.guildJoinResponse.Apply = apply;
            message.Request.guildJoinResponse.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildAdmini(object sender, GuildAdminResponse message) {
            Debug.LogFormat("OnGuildAdmini：{0} {1} ", message.Command, message.Result);
            MessageBox.Show(string.Format("执行操作：{0} 结果：{1} {2}", message.Command, message.Result, message.Errormsg));

        }

    }
}
