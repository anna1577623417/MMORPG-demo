using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class UIQuestDialog : UIWindow {
    [SerializeField]private UIQuestInfo questInfo;
    [SerializeField] private GameObject openButtons;
    [SerializeField] private GameObject submitButtons;

    public Quest quest;

    void Start() {

    }
    public void SetQuest(Quest quest) {
        this.quest = quest;
        this.UpdateQuest();
        if(this.quest.Info == null) {//若Info为null,则尚未从服务器返回信息，判定为新任务
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        } else {
            if(this.quest.Info.Status == SkillBridge.Message.QuestStatus.Completed) {
                openButtons.SetActive(false) ;
                submitButtons.SetActive(true) ;
            } else {
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }
    }

    private void UpdateQuest() {
        if(this.quest != null) {
            if(this.questInfo != null) {
                this.questInfo.SetQuestInfo(this.quest);
            }
        }
    }
}

