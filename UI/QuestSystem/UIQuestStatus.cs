using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Npc头顶任务状态的显示
/// </summary>
public class UIQuestStatus : UIWorldElement {
    public Image[] statusImages;

    private NpcQuestStatus questStatus;

    void Start() {

    }

    public void SetQuestStatus(NpcQuestStatus status) {
        this.questStatus = status;

        for(int i = 0; i< statusImages.Length; i++) {
            if (this.statusImages[i] != null) {
                this.statusImages[i].gameObject.SetActive(i == (int)status);
            }
        }
    }
}

