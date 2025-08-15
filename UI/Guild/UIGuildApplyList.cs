using Assets.Scripts.Services;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow {
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ListView listMain;
    [SerializeField] private Transform itemRoot;

    //打开列表时，如果恰好有新申请消息，也能刷新列表让其显示在列表中
    void Start () {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        this.UpdateList();
    }

    private void OnDestroy() {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    private void UpdateList() {
        ClearList();
        InitItems();
    }

    private void ClearList() {
        this.listMain.RemoveAll();
    }


    //可在ListView.ListViewItem写一个泛型虚函数
    private void InitItems() {
        foreach (var item in GuildManager.Instance.guildInfo.Applies) {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetItemInfo(item);
            this.listMain.AddItem(ui);
        }
    }
}
