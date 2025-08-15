using Assets.Scripts.Managers;
using Assets.Scripts.Models;
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
    class QuestService : Singleton<QuestService>, IDisposable {
        public QuestService() {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSumbit);
        }
        public void Dispose() {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSumbit);
        }

        public bool SendQuestAccept(Quest quest) {
            Debug.Log("SendQuestAccept");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message) {
            Debug.LogFormat("OnQuestAccept:{0}, ERR: {1}", message.Result, message.Errormsg);
            if(message.Result == Result.Success) {
                QuestManager.Instance.OnQuestAccpted(message.Quest);
            } else {
                MessageBox.Show("任务接受失败", "错误", MessageBoxType.Error);
            }
        }

        public bool SendQuestSumbit(Quest quest) {
            Debug.Log("SendQuestSumbit");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestSumbit(object sender, QuestSubmitResponse message) {
            Debug.LogFormat("OnQuestSumbit: {0}, ERR: {1}", message.Result, message.Errormsg);
            if(message.Result == Result.Success) {
                QuestManager.Instance.OnQuestSumited(message.Quest);
            } else {
                MessageBox.Show("任务完成 失败","错误",MessageBoxType.Error);
            }
        }

    }
}


