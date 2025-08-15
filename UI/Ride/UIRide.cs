using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow {

    [SerializeField] private Text description;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ListView listMain;
    public  UIRideItem selectedItem;

    void Start () {
        RefreshUI();
        this.listMain.onItemSelected += this.OnItemSelected;
        if (selectedItem != null) {
            this.description.text = "坐骑描述：";
        }
    }

    private void RefreshUI() {
        ClearItems();
        InitItems();
    }

    //use ItemManager to store and control the rides
    private void InitItems() {
        foreach (var kv in ItemManager.Instance.Items) {

            if(kv.Value.Define.Type == ItemType.Ride && 
                (kv.Value.Define.LimitClass == CharacterClass.None || kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)) {

                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetRideItem(kv.Value, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    private void ClearItems() {
        if(this.listMain!=null) this.listMain.RemoveAll();
    }

    public void OnItemSelected(ListView.ListViewItem item) { 
        this.selectedItem = item as UIRideItem;
        this.description.text = this.selectedItem.item.Define.Description;
    }

    public void DoRide() {
        if(this.selectedItem == null) {
            MessageBox.Show("请选择要召唤的坐骑", "提示");
            return;
        }
        Debug.Log("DoRide");
        User.Instance.Ride(this.selectedItem.item.Id);
    }

}
