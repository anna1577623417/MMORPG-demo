using Assets.Scripts.Models;
using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class UIQuestItem : ListView.ListViewItem {
    [SerializeField] private Text title;
    [SerializeField] private Image background;
    [SerializeField] private Sprite normalBG;
    [SerializeField] private Sprite selectedBG;

    public override void OnSelected(bool selected) {
        this.background.overrideSprite = selected?selectedBG:normalBG;
    }

    public Quest quest;

    bool isEquiped = false;

    public void SetQuestInfo(Quest item) {
        this.quest = item;
        string questTab = item.Define.Type == QuestType.Main ? "【主线】" : "【支线】";
        if (this.title != null) {
            this.title.text = questTab + this.quest.Define.Name;
        }
    }

}
