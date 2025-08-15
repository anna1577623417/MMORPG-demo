using Common.Data;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using Assets.Scripts.Managers;


namespace Managers {
    internal class NPCManager:Singleton<NPCManager> {
        public delegate bool NpcActionHandler(NpcDefine npcDefine);
        Dictionary<int,Vector3> npcPositions = new Dictionary<int,Vector3>();

        Dictionary<NpcDefine.NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcDefine.NpcFunction, NpcActionHandler>();

        public void RegisterNpcEvent(NpcDefine.NpcFunction function,NpcActionHandler action) {
            if(!eventMap.ContainsKey(function)) {
                eventMap[function] = action;
            }else {
                eventMap[function] += action;
            }
        }

        public bool Interactive(int npcId) {
            if(DataManager.Instance.NPCs.ContainsKey(npcId)) {
                var npc = DataManager.Instance.NPCs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        public bool Interactive(NpcDefine npc) {
            if(DoTaskInteractive(npc)) {//无论是否是任务类型的npc，都需要检测是否有任务，例如装备npc也派送任务
                return true;
            }else if(npc.Type== NpcDefine.NpcType.Functional) {//double check
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NpcDefine npc) {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None) return false;

            return QuestManager.Instance.OpenNpcQuest(npc.ID);
            //npc管理器仅靠这一行传入npcID并调用任务管理器的函数来实现逻辑
            //关联性低，后期维护成本小
        }

        private bool DoFunctionInteractive(NpcDefine npc) {
            if (npc.Type != NpcDefine.NpcType.Functional) {//double check
                return false;
            }
            if (!eventMap.ContainsKey(npc.Function)) {
                return false;
            }
            return eventMap[npc.Function](npc);
            //eventMap[npc.Function]=NpcActionHandler,npc = parameter
        }

        public NpcDefine GetNpcDefine(int NpcID) {

            return DataManager.Instance.NPCs[NpcID];
        }
        internal void UpdateNpcPosition(int npc,Vector3 position) {//设置位置
            this.npcPositions[npc] = position;
        }
        internal Vector3 GetNpcPosition(int npc) {//获取位置
            return this.npcPositions[npc];
        }
    }
}
