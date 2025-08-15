using Common;
using Models;
using SkillBridge.Message;
using UnityEngine;

namespace Managers {
    internal class GuildManager : Singleton<GuildManager> {
        public NGuildInfo guildInfo;

        public NGuildMemberInfo myGuildMemberInfo;
        public bool HasGuild {
            get { return guildInfo != null; }
        }

        public void Init(NGuildInfo guild) {
            this.guildInfo = guild;
            if(guild == null) {
                myGuildMemberInfo = null;
                return;
            }
            foreach(var member in guildInfo.Members) {
                if(member.characterId == User.Instance.CurrentCharacter.Id) {
                    myGuildMemberInfo = member;
                    break;
                } 
            }
        }
        internal void ShowGuild() {
            if(this.HasGuild) {
                UIManager.Instance.Show<UIGuild>();
            } else {
                var win = UIManager.Instance.Show<UIGuildPopNotifyGuild>();
                if (win != null) {
                    win.OnClose += UIGuild_OnClose;
                } else {
                    Debug.LogError("ShowGuild,win = null");
                }

            }
        }

        private void UIGuild_OnClose (UIWindow sender,UIWindow.WindowResult result) {
            if(result == UIWindow.WindowResult.Yes) {//创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();
            }else if(result == UIWindow.WindowResult.No){//加入公会
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
