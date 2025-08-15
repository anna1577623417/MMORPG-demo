using Models;
using SkillBridge;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    public class BagManager : Singleton<BagManager> {
        //byte data saved in server is sent to client in which the data will be transformed into the data  structure we need in client
        public int Unlocked;

        public BagItem[] Items;

        public NBagInfo Info;

        unsafe public void Init(NBagInfo info) {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            //naturally,the Items.Lenght eauqls the number of Unlocked Cells
            if(info.Items!=null&& info.Items.Length >= this.Unlocked) {
                Analyze(info.Items);
                Debug.Log("Items.Lenght: " + Items.Length);
            } else {
                Info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
        }

        public void ShowBag() {
            UIBag uiBag = UIManager.Instance.Show<UIBag>();
            uiBag.moneyText.text= User.Instance.CurrentCharacter.Gold.ToString();//set the money
            uiBag.SetBagView();//set the scroll bar value 
        }

        //check again
        //recognize the items and itergrate item as neccessary
        public void Reset() {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items) {
                if (kv.Value.Count <= kv.Value.Define.StackLimit) {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count; 
                } else {
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit) {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;//move on to next cell
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;//move on to next cell
            }
        }

        // pointer in C#
        unsafe void Analyze(byte[] data) {
            fixed (byte*pt = data) {
                for(int i=0;i<this.Items.Length; i++) {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));//make item point to ith element in data array
                    Items[i] = *item;//value type can drectly be modified value
                    //input byte array and generate a BagItem array 
                }
            }
        }

        unsafe public NBagInfo GetBageInfo() {
            fixed (byte*pt = Info.Items) {
                for(int i = 0; i < this.Unlocked; i++) {
                    BagItem*item = (BagItem*)(pt+i* sizeof(BagItem)); 
                    *item = Items[i];//to get byte array(memory) from BagItem array
                }
            }
            return this.Info;
        }

        //背包格子满了的情况没做处理
        internal void AddItem(int itemId, int count) {
            ushort addCount = (ushort)count;
            ushort StackLimit = (ushort)DataManager.Instance.Items[itemId].StackLimit;
            for (int i = 0; i < Items.Length; i++) {
                if (this.Items[i].ItemId == itemId) {
                    ushort canAdd = (ushort)(StackLimit - this.Items[i].Count);
                    if (canAdd >= addCount) {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    } else {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
         
            if (addCount > 0) {
                for (int i = 0; i < Items.Length; i++) {
                    if (this.Items[i].ItemId == 0) {//this.Items[i].ItemId == 0空白格子
                        this.Items[i].ItemId = (ushort)itemId;
                        this.Items[i].Count = addCount;
                        break;//avoid item bought fill the all cells
                    }
                }
            }
        }

        //以下是处理StackLimit=1的情况
        //由于上述情况canAdd始终为0所以不会加上去

        //这种情况可能只能适用于每次购买1个道具的情况，因为这里没有使addCount减少的情况
        //例如，购买3个不可堆叠的东西，那么需要找到3个空白空白格子，而这里直接使this.Items[i].Count = addCount;
        //只有默认addcount=1才能正常显示
        internal void RemoveItem(int itemId, int count) {
            
        }
    }
}

