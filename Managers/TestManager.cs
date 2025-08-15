using Common.Data;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers {
    internal class TestManager:Singleton<TestManager> {

        public void Init() {
            //NPCManager.Instance.RegisterNpcEvent(NpcDefine.NpcFunction.InvokeShop, OnNpcInvokeShop);
            //NPCManager.Instance.RegisterNpcEvent(NpcDefine.NpcFunction.InvokeDungeon, OnNpcInvokeDungeon);
        }

        private bool OnNpcInvokeShop(NpcDefine Npc) {
            Debug.LogFormat("TestManager.OnNpcInvokeShop：NPC: {0} :{1} Type: {2} Func: {3} ", Npc.ID, Npc.Name, Npc.Type, Npc.Function);
            UITest test = UIManager.Instance.Show<UITest>();
            test.SetTitle(Npc.Name);
            return true;
        }

        private bool OnNpcInvokeDungeon(NpcDefine Npc) {
            Debug.LogFormat("TestManager.OnNpcInvokeShop：NPC: {0} :{1}  Type: {2} Func: {3} ", Npc.ID, Npc.Name, Npc.Type, Npc.Function);
            MessageBox.Show("clieked the Npc：" + Npc.Name, "NPC Conversation");
            return true;
        }
    }
}
