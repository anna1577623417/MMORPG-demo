using Assets.Scripts.Models;
using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.Scripts.Managers {

    public enum NpcQuestStatus {
        None = 0,//无任务
        Available,//拥有可接受任务
        Complete,//拥有已完成可提交任务
        Incomplete,//拥有未完成任务
    }

    public class QuestManager : Singleton<QuestManager> {
        //所有有效任务,由服务器回调并在客户端初始化得到quests
        public List<NQuestInfo> questInfos;//存储关于任务的网络信息的容器
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();//总任务列表

        //查找npc身上的任务,NpcQuestStatus查找任务状态，List<Quest>查询任务列表 
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> QuestStatusChanged;
        public void Init(List<NQuestInfo> quests) {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        private void InitQuests() {
            //初始化已有任务
            foreach (var info in this.questInfos) {
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests) {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        private void CheckAvailableQuests() {
            //初始化可用任务
            foreach (var kv in DataManager.Instance.Quests) {

                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class) continue;//职业不符合

                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level) continue;//等级不符合

                if (this.allQuests.ContainsKey(kv.Key)) continue;//任务已存在

                //检查前置任务
                if (kv.Value.PreQuest > 0) {
                    Quest prequest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out prequest)) {//获取前置任务
                        if (prequest.Info == null) continue;//存在前置任务但未接取

                        if (prequest.Info.Status != QuestStatus.Finished) continue;//前置任务接取了但未完成
                    } else {
                        continue;//前置任务还没接取
                    }
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest) {
            if (!this.npcQuests.ContainsKey(npcId)) {
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }

            List<Quest> availables;
            List<Quest> completes;
            List<Quest> incompletes;

            //三个if分别初始化任务状态
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables)) {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out completes)) {
                completes = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = completes;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incompletes)) {
                incompletes = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incompletes;
            }
            //判断任务状态
            if (quest.Info == null) {
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest)) {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            } else {
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Completed) {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest)) {
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress) {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest)) {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }
            }
        }

        /// <summary>
        /// 获取NPC任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId) {

            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>() { };
            //严格按照已完成，可接受，未完成任务来处理
            if (this.npcQuests.TryGetValue(npcId, out status)) {//获取npc任务
                if (status[NpcQuestStatus.Complete].Count > 0) return NpcQuestStatus.Complete;

                if (status[NpcQuestStatus.Available].Count > 0) return NpcQuestStatus.Available;

                if (status[NpcQuestStatus.Incomplete].Count > 0) return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId) {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>() { };
            if (this.npcQuests.TryGetValue(npcId, out status)) {//获取npc任务
                if (status[NpcQuestStatus.Complete].Count > 0) return ShowQuestDialog(status[NpcQuestStatus.Complete].First());

                if (status[NpcQuestStatus.Available].Count > 0) return ShowQuestDialog(status[NpcQuestStatus.Available].First());

                if (status[NpcQuestStatus.Incomplete].Count > 0) return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }

            return false;
        }
        
        private bool ShowQuestDialog(Quest quest) {
            if (quest.Info == null || quest.Info.Status == QuestStatus.Completed) {
                UIQuestDialog dialog = UIManager.Instance.Show<UIQuestDialog>();
                dialog.SetQuest(quest);
                dialog.OnClose += OnQuestDialogClose;//绑定父类UIWindow的处理关闭事件的委托
                return true;
            }
            if (quest.Info != null || quest.Info.Status == QuestStatus.Completed) {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        //处理任务对话的函数，包含接受任务，拒绝任务，任务完成的npc的对话
        private void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result) {
            UIQuestDialog dialog = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes) {//接受任务和提交任务都是yes
                if (dialog.quest.Info == null) {//接任务的判定条件
                    QuestService.Instance.SendQuestAccept(dialog.quest);//接受任务的会话
                } else if (dialog.quest.Info.Status == QuestStatus.Completed) {//完成任务的判定方法
                    QuestService.Instance.SendQuestSumbit(dialog.quest);//
                }
            } else if (result == UIWindow.WindowResult.No) {//拒绝任务和关闭UI
                MessageBox.Show(dialog.quest.Define.DialogDeny);
            }
        }
        public void OnQuestAccpted(NQuestInfo info) {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSumited(NQuestInfo info) {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }

        /// <summary>
        /// 刷新任务状态
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        private Quest RefreshQuestStatus(NQuestInfo quest) {
            this.npcQuests.Clear();
            Quest result;
            if (this.allQuests.ContainsKey(quest.QuestId)) {
                //更新任务状态
                this.allQuests[quest.QuestId].Info = quest;
                result = this.allQuests[quest.QuestId];
            } else {
                result = new Quest(quest);
                this.allQuests[quest.QuestId] = result;
            }

            CheckAvailableQuests();

            foreach (var kv in this.allQuests) {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            if (QuestStatusChanged != null) QuestStatusChanged(result);
            return result;
        }
    }
}
