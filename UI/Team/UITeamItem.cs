using Managers;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem {
    [SerializeField] private Text nickName;
    [SerializeField] private Text classtext;
    [SerializeField] private Image classIcon;
    [SerializeField] private Image leaderIcon;

    [SerializeField] private Image background;

    public override void OnSelected(bool selected) {
        this.background.enabled = selected ? true : false;
    }

    public int index;
    public NCharacterInfo Info;

    void Start() {
        this.background.enabled = false;
    }

    public void SetMemberInfo(int index, NCharacterInfo item , bool isLeader) {
        this.index = index;
        this.Info = item;
        if(this.nickName != null) this.nickName.text = this.Info.Level.ToString().PadRight(4) + this.Info.Name;
        if (this.classtext != null) {
            var ChracterClass = (GameDefine.ClassType)this.Info.Class;
            this.classtext.text = GameDefine.GetDescription(ChracterClass);
        } 
        if (this.classIcon !=null)this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.Info.Class-1];
        if(this.leaderIcon!=null) this.leaderIcon.gameObject.SetActive(isLeader);
    }

}
