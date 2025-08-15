using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem {
    [SerializeField] private Text nickName;
    [SerializeField] private Text @class;
    [SerializeField] private Text level;
    [SerializeField] private Text status;

    [SerializeField] private Image backGround;
    [SerializeField] private Sprite normalBG;
    [SerializeField] private Sprite selectedBG;

    [SerializeField] private Color OnlineColor;
    [SerializeField] private Color OfflineColor;
    public override void OnSelected(bool isSelected) {
        this.backGround.overrideSprite = isSelected ? selectedBG : normalBG;
    }
    public NFriendInfo Info;
    void Start() {
    }
    bool isEquiped = false;

    public void SetFriendInfo(NFriendInfo item) {
        this.Info = item;
        if (this.nickName != null) this.nickName.text = this.Info.friendInfo.Name;
        if(this.@class !=null) this.@class.text = this.Info.friendInfo.Class.ToString();
        if(this.level !=null)   this.level.text = this.Info.friendInfo.Level.ToString();
        if (this.status != null) {
            this.status.text = this.Info.Status == 1 ? "在线" : "离线";
            this.status.color = this.Info.Status == 1 ? OnlineColor : OfflineColor;
        }
    }
}
