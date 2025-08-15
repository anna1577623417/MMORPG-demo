using Assets.Scripts.Models;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

    [SerializeField] private Text title;

    [SerializeField] private Text[] targets;

    [SerializeField]private Text description;
    [SerializeField] private Text TargetText;

    [SerializeField] private Text overview;
    public UIIconItem rewardItems;
    [SerializeField] private Text rewardMoney;
    [SerializeField] private Text rewardExp;

    public Button navButton;
    private int npc = 0;

    void Start() {

    }

    public void SetQuestInfo(Quest quest) {
        this.title.text = string.Format("[{0}] {1}",quest.Define.Type,quest.Define.Name);
        if (this.overview != null) this.overview.text = quest.Define.Overview;
        if(this.description != null) {
            if (quest.Info == null) {
                this.description.text = quest.Define.Dialog;
            } else {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Completed) {
                    this.description.text = quest.Define.DialogFinish;
                }
            }
        }

        //目标文本的处理？

        //任务奖励的金币和经验
        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        if(quest.Info == null) {//确定角色该找哪个npc
            this.npc = quest.Define.AcceptNPC;
        }else if(quest.Info.Status == QuestStatus.Completed){
          this.npc = quest.Define.SubmitNPC;
        }
        this.navButton.gameObject.SetActive(this.npc > 0);//根据是否可接来确定是否显示按钮

        foreach(var fitter in this.GetComponentsInChildren<ContentSizeFitter>()) {
            fitter.SetLayoutVertical();//自适应，刷新布局对齐
        }
    }

    public void OnClickAbandon() {

    }

    public void OnClickNav() {
        Vector3 pos =  NPCManager.Instance.GetNpcPosition(this.npc);//获取位置
        User.Instance.CurrentCharacterObject.StartNav(pos);//开启寻路（协程）
        UIManager.Instance.Close<UIQuestSystem>();
    }
}
