using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models {
    public class Item {

        public int Id;
        public int Count;
        public ItemDefine Define;//refer the field in ItemDfine table
        public EquipDefine EquipInfo;

        //reload this constructor
        //use NItemInfo to construct a item
        public Item(NItemInfo item) :
            this(item.Id, item.Count) { 
        }
        //we can use id and Count to construct a item but a network info
        public Item(int id ,int Count) {
            this.Id = id;
            this.Count = Count;
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            //this.Define = DataManager.Instance.Items[this.Id];
            DataManager.Instance.Equips.TryGetValue(this.Id , out this.EquipInfo);
        }

        public override string ToString() {
            return string.Format("Id:{0}, Count: {1},",this.Id,this.Count);
        }
    }
}
