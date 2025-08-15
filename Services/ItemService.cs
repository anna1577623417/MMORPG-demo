using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

namespace Services {
    public class ItemService : Singleton<ItemService>,IDisposable {
        public ItemService() {
            //when client get response,OnItemBuy will be executed
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        Item pendingEquip = null;
        bool isEquip;
        public bool SendEquipItem(Item equip,bool isEquip) {
            if (pendingEquip != null) return false;

            Debug.Log("SendEquipItem");
            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnItemEquip(object sender, ItemEquipResponse message) {
            if(message.Result == Result.Success) {
                if(pendingEquip != null) {
                    if (this.isEquip) {
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    } else {
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    }
                    pendingEquip = null;
                }
            }
        }

        public void Dispose() {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        //directly send the NetMessage to server
        public void SendBuyItem(int shopId,int shopItemId) {
            Debug.Log("SendBuyItem");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(message);
        }
        //handling the response from server
        //make a pop-up message
        private void OnItemBuy(object sender,ItemBuyResponse message) {
            MessageBox.Show("购买结果: " + message.Result + "\n" + message.Errormsg, "购买完成");
        }
    }
}


