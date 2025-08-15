using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterEquip : UIWindow {
	public Text title;
	public Text money;

	public GameObject itemPrefab;
	public GameObject itemEquipedPrefab;

	public Transform itemListRoot;

	public List<Transform> slots;

	void Start () {
		RefreshUI();
		EquipManager.Instance.OnEquipChanged += RefreshUI;

    }

	private void OnDestroy() {
		EquipManager.Instance.OnEquipChanged -= RefreshUI;
	}

    //this UI exist two list fo equip item ,that is on and off
    private void RefreshUI() {
		ClearAllEquipList();//list of equip item that is off showing in the left UI list
		InitAllEquipItems();
		ClearEquipedList();//list of equip item that is on 
        InitAllEquipedItems();
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }


    private void InitAllEquipItems() {
        foreach(var kv in ItemManager.Instance.Items) {
            if(kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class) {
                //conceal equip item has been on
                if (EquipManager.Instance.Contains(kv.Key)) continue;
                GameObject go = Instantiate(itemPrefab,itemListRoot);//generate it under itemListRoot 
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    private void ClearAllEquipList() {
        foreach(var item in itemListRoot.GetComponentsInChildren<UIEquipItem>()) {
            Destroy(item.gameObject);
        }
    }

    private void ClearEquipedList() {
        foreach (var item in slots) {
            if (item.childCount > 1) {
                Destroy(item.GetChild(1).gameObject);//this statement may  destroy title text under Equip Slot
            }
        }
    }
    private void InitAllEquipedItems() {
        for(int i = 0;i < (int)EquipSlot.SlotMax;i++) {
            var item = EquipManager.Instance.Equips[i];
            if(item != null) {
                GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(i,item,this,true);
            }
        }
    }

    public void DoEquip(Item item) {
        EquipManager.Instance.EquipItem(item);
    }
   
    public void UnEquip(Item item) {
        EquipManager.Instance.UnEquipItem(item);
    }
   

}
