using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem {

    public NGuildInfo Info;
    [SerializeField] Text Id;
    [SerializeField] Text Name;
    [SerializeField] Text leader;
    [SerializeField] Text quantity;

    [SerializeField] Image background;
    [SerializeField] Sprite normalBG;
    [SerializeField] Sprite selectedBG;

    public override void OnSelected(bool isSelected) {
        this.background.overrideSprite = isSelected ? selectedBG : normalBG;
    }

    internal void SetGuildInfo(NGuildInfo item) {
        Info = new NGuildInfo();
        Info = item;
        if(this.Id != null) { this.Id.text = Info.Id.ToString(); }
        if (this.Name != null) { this.Name.text = Info.GuildName; }
        if(this.leader != null) { this.leader.text = Info.leaderName; }
        if(this.quantity != null) { this.quantity.text = string.Format("{0}/{1}", Info.memberCount.ToString(), GameDefine.GuildMaxMemberCount); }

    }

}
