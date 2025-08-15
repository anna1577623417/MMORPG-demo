using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Managers {
    internal class EquipManager : Singleton<EquipManager> {

        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChanged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];//use this array to maintain character's equipment on and off

        byte[] Data; //int list

        public UIEquipItem PrevSelectedEquip = null;

        public void HightlightSwitch(UIEquipItem newSelected) {
            // 取消上一个物体的高亮交给UIEquipItem，在UIEquipItem那边置空PrevSelectedEquip=null即可
            if (PrevSelectedEquip != null) {
                PrevSelectedEquip.Selected = false;
            }
            // 更新当前选中物体（高亮由外部触发）
            PrevSelectedEquip = newSelected;
        }

        //应从网络端获取数据初始化人物已经穿上的装备
        unsafe public void Init(byte[] data) {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId) {
            for(int i = 0; i < this.Equips.Length; i++) {
                if (Equips[i] != null && Equips[i].Id == equipId) 
                    return true;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot) {
            return Equips[(int)slot];
        }

        //Serialization ,from object array to byte array
        unsafe public byte[] GetEquipData() {
            fixed (byte* pt = Data) {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++) {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null) {
                        *itemid = 0;
                    } else {
                        *itemid = Equips[i].Id;
                    }
                }
            }
            return this.Data;
        }
        //Deserialization,from  one object array to  another object array
        unsafe void ParseEquipData(byte[] data) {
            fixed (byte *pt = this.Data) {
                for(int i = 0;i< this.Equips.Length;i++) {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0) {
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    } else {
                        Equips[i] = null;
                    }
                }
            }
        }

        public void EquipItem(Item equip) {
            ItemService.Instance.SendEquipItem(equip,true);
        }

        public void UnEquipItem(Item equip) {
            ItemService.Instance.SendEquipItem(equip,false);
        }

        internal void OnEquipItem(Item equip) {
            //要穿的装备和已经穿上的装备是同一件，则不作任何处理，直接返回
            if (this.Equips[(int)equip.EquipInfo.Slot]!=null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id) {
                return;
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];
            //直接替换装备卡槽装备的语句
            //get the equipitem from ItemManager and add it to Equips (equip cells)

            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        internal void OnUnEquipItem(EquipSlot slot) {
            if (this.Equips[(int)slot] != null) {
                this.Equips[(int)slot] = null;//若不为空，直接置空
                if(OnEquipChanged != null) OnEquipChanged();
            }
        }
    }

}
