using Common.Data;
using Models;
using SkillBridge.Message;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Managers {
    public class ItemManager : Singleton<ItemManager> {

        public Dictionary<int,Item> Items = new Dictionary<int,Item>();
            
        internal void Init(List<NItemInfo> List_NItems) {
            this.Items.Clear();
            foreach(var info in List_NItems) {
                Item item = new Item(info);//generate  NItemInfo from the Items of List via Network
                this.Items.Add(item.Id,item);

                Debug.LogFormat("ItemManager: Init[{0}]", item);

            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
            //Register StatusNotifyHandler in StatusService
        }

        private bool OnItemNotify(NStatus status) {
            if(status.Action == StatusAction.Add) {
                this.AddItem(status.Id,status.Value);
            }
            if(status.Action == StatusAction.Delete) {
                this.RemoveItem(status.Id,status.Value);
            }
            UIManager.Instance.UpdateBagItem();//refresh BagItem
            return true;
        }
        private void AddItem(int itemId, int count) {
            Item item = null;
            if(this.Items.TryGetValue(itemId,out item)) {
                item.Count += count;
            } else {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
                //directly add a item into Items,where items message is saved in this  dictionary 

            }
            BagManager.Instance.AddItem(itemId,count);
        }

        private void RemoveItem(int itemId, int count) {
            if (!this.Items.ContainsKey(itemId)){
                return;
            }
            Item item = this.Items[itemId];
            if (item.Count < count) 
                return;
            item.Count -= count;
            BagManager.Instance.RemoveItem(itemId,count);
        }

            public ItemDefine GetItem(int ItemId) {
            return null;
        }

        public bool UseItem(int ItemId) {
            return false;
        }

        public bool UseItem(ItemDefine item) {
            return false ;
        }
    }
}

