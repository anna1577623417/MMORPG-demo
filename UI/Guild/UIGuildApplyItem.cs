using Assets.Scripts.Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//这个列表项的类没有重写OnSelected，因为这个列表项不需要选中状态
public class UIGuildApplyItem : ListView.ListViewItem {

    [SerializeField] private Text nickname;
    [SerializeField] private Text _class;
    [SerializeField] private Text level;

    public NGuildApplyInfo Info;

    internal void SetItemInfo(NGuildApplyInfo item) {
        this.Info = item;
        if(this.nickname != null) this.nickname.text = this.Info.Name;
        if(this._class != null) this._class.text = ((GameDefine.ClassType)this.Info.Class).ToString();
        if(this.level != null) this.level.text = this.Info.Level.ToString();
    }

    public void OnAccept() {
        MessageBox.Show(string.Format("确认同意【{0}】的公会申请?", this.Info.Name), "审批申请", MessageBoxType.Confirm).OnYes = () => {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    public void OnDecline() {
        MessageBox.Show(string.Format("确认拒绝【{0}】的公会申请?", this.Info.Name), "审批申请", MessageBoxType.Confirm).OnYes = () => {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}
