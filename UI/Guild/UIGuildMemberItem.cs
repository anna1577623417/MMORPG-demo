using SkillBridge.Message;
using Common.Utils;
using Common.Data;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem {
    [SerializeField] Text nickName;
    [SerializeField] Text _class;
    [SerializeField] Text level;
    [SerializeField] Text Position;
    [SerializeField] Text joinTime;
    [SerializeField] Text status;

    [SerializeField] Image background;
    [SerializeField] Sprite normalBG;
    [SerializeField] Sprite selectedBG;

    [SerializeField] private Color OnlineColor;
    [SerializeField] private Color OfflineColor;

    public override void OnSelected(bool isSelected) {
        this.background.overrideSprite = isSelected ? selectedBG : normalBG;
    }

    public NGuildMemberInfo GuildMemberInfo;

    public void SetGuildMemberInfo(NGuildMemberInfo item) {
        this.GuildMemberInfo = item;
        if(this.nickName != null) this.nickName.text = this.GuildMemberInfo.Info.Name;
        if(this._class !=null)  this._class.text = this.GuildMemberInfo.Info.Class.ToString();
        if(this.level !=null) this.level.text = this.GuildMemberInfo.Info.Level.ToString();
        if(this.Position!=null){
            var position = (GameDefine.GuildPosition)this.GuildMemberInfo.Title;
            this.Position.text = GameDefine.GetDescription(position);
        } 
        if (this.joinTime != null) this.joinTime.text = TimeUtil.GetTime(this.GuildMemberInfo.joinTime).ToShortDateString();
        if (this.status != null) {
            this.status.text = this.GuildMemberInfo.Status == 1 ? "在线" : "离线";
            this.status.color = this.GuildMemberInfo.Status == 1 ? OnlineColor : OfflineColor;
        }
    }

}
