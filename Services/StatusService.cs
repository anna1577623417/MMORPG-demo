using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Services;
using SkillBridge.Message;
using Network;
using Managers;


namespace Services {
    public class StatusService : Singleton<StatusService> ,IDisposable{
        public delegate bool StatusNotifyHandler(NStatus status);

        Dictionary<StatusType,StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler> ();
        HashSet<StatusNotifyHandler> handlers = new HashSet<StatusNotifyHandler>();
        
       public void Init() {

        }

        public void RegisterStatusNotify(StatusType function,StatusNotifyHandler action) {

            if (handlers.Contains(action)) return;

            //avoid action being added to this event multiply tiems
            if (!eventMap.ContainsKey(function)) {
                eventMap[function] = action;
            } else {
                eventMap[function] += action;
            }

            handlers.Add(action);
        }

        public StatusService() {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Dispose() {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        //iterate this status protocal
        //status may include money changed or item changed
        private void OnStatusNotify(object sender,StatusNotify notify) {
            foreach(NStatus status in notify.Status) {
                Notify(status); 
            }
        }

        private void Notify(NStatus status) {
            Debug.LogFormat("StatusNotify: [ {0} {1} {2} {3}]",status.Type,status.Action,status.Id,status.Value);

            //this block directly modifies money
            if(status.Type == StatusType.Money) {
                if(status.Action ==StatusAction.Add) {
                    User.Instance.AddGold(status.Value);
                }else if (status.Action == StatusAction.Delete) {
                    User.Instance.AddGold(-status.Value);
                }
                UIManager.Instance.UpdateMoney();
            }

            //if stauts type is not money,send a notifinication
            //other class may register the StatusNotifyHandler,if not, skip it
            StatusNotifyHandler handler;
            if(eventMap.TryGetValue(status.Type, out handler)) {
                handler(status);
            }
        }
    }
}

