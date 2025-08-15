using Managers;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIGuild : UIWindow {
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ListView listMain;
    [SerializeField] private Transform itemRoot;
    [SerializeField] private UIGuildInfo uiInfo;
    [SerializeField] private UIGuildMemberItem selectedItem;

    [SerializeField] private GameObject panelAdmin;
    [SerializeField] private GameObject panelLeader;

    void Start () {
        GuildService.Instance.OnGuildUpdate += UpdateUI;//不独占则需要添加
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }
    void OnDestroy() {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;//销毁时取消订阅
    }

    void UpdateUI() {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;

        ClearList();
        InitItems();

        this.panelAdmin.SetActive(GuildManager.Instance.myGuildMemberInfo.Title > GuildTitle.None);
        this.panelLeader.SetActive(GuildManager.Instance.myGuildMemberInfo.Title == GuildTitle.President);
    }
    public void OnGuildMemberSelected(ListView.ListViewItem item) {
        this.selectedItem = item as UIGuildMemberItem;
    }

    /// <summary>
    /// 初始化所有公会列表
    /// </summary>
    private void InitItems() {
        foreach(var item in GuildManager.Instance.guildInfo.Members) {
            GameObject go = Instantiate(itemPrefab,this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList() {
        this.listMain.RemoveAll();
    }

    public void OnClickAppliesList() {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    public void OnClickLeave() {

    }

    public void OnClickChat() {
        if (selectedItem == null) {
            MessageBox.Show("选择要聊天的成员");
            return;
        }
    }

    public void OnClickKickout() {
        if(selectedItem == null) {
            MessageBox.Show("选择要踢出的成员");
            return;
        }

        MessageBox.Show(string.Format("要将【{0}】踢出公会吗？", this.selectedItem.GuildMemberInfo.Info.Name, "踢出公会", MessageBoxType.Confirm, "确定", "取消")).OnYes = () => {
            GuildService.Instance.SendAdminiCommand(GuildAdminCommand.Kictout,this.selectedItem.GuildMemberInfo.Info.Id);
        };
    }

    public void OnClickPromote() {
        if(selectedItem == null) {
            MessageBox.Show("请选择要晋升的成员");
            return;
        }
        if(selectedItem.GuildMemberInfo.Title != GuildTitle.None) {
            MessageBox.Show("对方已经是{0}", ((GameDefine.GuildPosition)selectedItem.GuildMemberInfo.Title).ToString());
        }

        MessageBox.Show(string.Format("要晋升【{0}】为公会副会长吗",this.selectedItem.GuildMemberInfo.Info.Name),"晋升",MessageBoxType.Confirm,"确定","取消").OnYes=()=>{
            GuildService.Instance.SendAdminiCommand(GuildAdminCommand.Promote, this.selectedItem.GuildMemberInfo.Info.Id);
        };
    }

    public void OnClickDepose() {
        if(selectedItem == null) {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }

        if(selectedItem.GuildMemberInfo.Title == GuildTitle.None) {
            MessageBox.Show("该成员没有任何职务");
            return;
        }
        if(selectedItem.GuildMemberInfo.Title == GuildTitle.President) {
            MessageBox.Show("无权免职会长");
            return;
        }
        MessageBox.Show(string.Format("要罢免【{0}】的公会职务吗?", this.selectedItem.GuildMemberInfo.Info.Name), "罢免职务", MessageBoxType.Confirm, "确定", "取消").OnYes =()=>{
            GuildService.Instance.SendAdminiCommand(GuildAdminCommand.Demote, this.selectedItem.GuildMemberInfo.Info.Id);
        };
    }

    public void OnClickTransfer() {
        if(selectedItem == null) {
            MessageBox.Show("请选择要会长职务转让的目标成员");
            return;
        }
        MessageBox.Show(string.Format("【确定】要把会长转让给【{0}】吗?", this.selectedItem.GuildMemberInfo.Info.Name), "转移会长", MessageBoxType.Confirm, "确定", "取消").OnYes=()=> { 
                GuildService.Instance.SendAdminiCommand(GuildAdminCommand.Transfer,this.selectedItem.GuildMemberInfo.Info.Id);
        };
    }


    public void OnClickSetNotice() {
        MessageBox.Show("扩展作业");
    }


}
