using Assets.Scripts.Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow {
    [SerializeField] GameObject itemPrefab;
    [SerializeField] ListView listMain;
    [SerializeField] Transform itemRoot;
    [SerializeField] UIGuildInfo uiInfo;
    [SerializeField] UIGuildItem selectedItem;

    void Start () {
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;//分页刷新

        GuildService.Instance.SendGuildListRequest();//公会列表刷新
    }

    private void OnDestroy() {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
        this.listMain.onItemSelected -= this.OnGuildMemberSelected;
    }

    void UpdateGuildList(List<NGuildInfo> guilds) {
        ClearList();
        InitItems(guilds);
    }

    private void ClearList() {
        this.listMain.RemoveAll();
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item) {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    /// <summary>
    /// 初始化所有列表项
    /// </summary>
    /// <param name="guilds"></param>
    void InitItems(List<NGuildInfo> guilds) {
        foreach(var item in guilds) {
            GameObject go = Instantiate(itemPrefab,this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }
    public void OnclickJoin() {
        if(selectedItem == null) {
            MessageBox.Show("请选择要加入的公会");
            return;
        } 
        MessageBox.Show(string.Format("确定要加入公会【{0}】吗?", selectedItem.Info.GuildName), "申请加入公会", MessageBoxType.Confirm, "申请加入", "取消").OnYes=()=>{
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
        };
    }
}
