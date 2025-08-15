using Assets.Scripts.Services;
using Entities;
using Models;
using SkillBridge;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;

namespace Managers {
    public class ChatManager : Singleton<ChatManager> {

        public enum LocalChannel {
            All = 0,//所有
            Local = 1,//本地
            World = 2,//世界
            Team = 3,//队伍
            Guild = 4,//公会
            Private = 5,//私聊
        }

        private ChatChannel[] ChannelFilter = new ChatChannel[6] {
            ChatChannel.Local | ChatChannel.World |  ChatChannel.Guild | ChatChannel.Team | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
        };

        //one channel for one list container
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6] {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        public LocalChannel displayChannel;//active channel

        public LocalChannel sendChannel;

        public int PrivateID = 0;
        public string PrivateName = "";

        public ChatChannel SendChannel {
            get {
                switch(sendChannel) {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }

        public Action OnChat { get;internal set; }

        public void Init() {
            foreach(var messages in this.Messages) {
                messages.Clear();
            }
        }


        //raise a private chat
        internal void StartPrivateChat(int targetId, string targetName) {
            this.PrivateID = targetId;
            this.PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;
            this.OnChat?.Invoke();
        }


        internal void SendChat(string content, int toId, string toName = "") {
            ChatService.Instance.SendChat(this.SendChannel,content, toId, toName);
        }

        internal bool SetSendChannel(LocalChannel Channel) {
            if(Channel == LocalChannel.Team) {
                if(User.Instance.TeamInfo == null) {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }
            if(Channel == LocalChannel.Guild) {
                if(User.Instance.CurrentCharacter.Guild == null) {
                    this.AddSystemMessage("你没有加入任何公会");
                    return false;
                }
            }
            this.sendChannel = Channel;
            Debug.LogFormat("Set Channel : {0}", this.sendChannel);
            return true;
        }

        internal void AddMessages(ChatChannel channel,List<ChatMessage> messages) {
            for(int i=0; i < 6; i++) {
                if ((this.ChannelFilter[i] & channel) == channel) {//bitwise operation and '&'
                    this.Messages[i].AddRange(messages);
                }
            }
            this.OnChat?.Invoke();
        }

        public void AddSystemMessage(string message,string from = "") {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage() {//Messages is a 2-dimension list 
                Channel = ChatChannel.System,
                Message = message,
                FromName=from
            });
            this.OnChat?.Invoke();
        }

        public string GetCurrentMessages() {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var message in this.Messages[(int)displayChannel]) {//get the shwon channel messages 
                stringBuilder.AppendLine(FormatMessage(message));//add and change the line
            }
            return stringBuilder.ToString();
        }

        private string FormatMessage(ChatMessage message) {
            // 添加消息内容转义
            string escapedMessage = TmpEscapeUtility.EscapeXml(message.Message);

            switch (message.Channel) {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(message), escapedMessage);
                case ChatChannel.World:
                    return string.Format("<color=#00FFFF>[世界]{0}{1}</color>", FormatFromPlayer(message), escapedMessage);
                case ChatChannel.System:
                    return string.Format("<color=#FFFF00>[系统]{0}</color>", escapedMessage);
                case ChatChannel.Private:
                    return string.Format("<color=#FF00FF>[私聊]{0}{1}</color>", FormatFromPlayer(message), escapedMessage);
                case ChatChannel.Team:
                    return string.Format("<color=#00FF00>[队伍]{0}{1}</color>", FormatFromPlayer(message), escapedMessage);
                case ChatChannel.Guild:
                    return string.Format("<color=#4169E1>[公会]{0}{1}</color>", FormatFromPlayer(message), escapedMessage);
            }
            return "";
        }

        private string FormatFromPlayer(ChatMessage message) {
            if (message.FromId == User.Instance.CurrentCharacter.Id) {
                // 修正后的超链接格式，颜色包裹在link外层
                return "<color=#00FF00><link=\"player:self\">[我]</link></color>";
            } else {
                // 标准超链接格式，带玩家ID参数
                return string.Format(
                    "<color=#87CEEB><link=\"player:{0}\">[{1}]</link></color>",
                    message.FromId,
                    TmpEscapeUtility.EscapeXml(message.FromName) // 转义特殊字符
                );
            }
        }


    }

}

