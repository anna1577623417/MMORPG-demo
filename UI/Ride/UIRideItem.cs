using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using UnityEngine.UI;
using Models;
using System.Text;

public class UIRideItem : ListView.ListViewItem {

    [SerializeField] private Image icon;
    [SerializeField] private Text title;
    [SerializeField] private Text level;

    [SerializeField] private Image background;
    [SerializeField] private Sprite normalBG;
    [SerializeField] private Sprite selectedBG;

    public override void OnSelected(bool selected) {
        this.background.overrideSprite = selected ? selectedBG : normalBG;
        if (!selected ) {
            this.owner.selectedItem = null;
        }
    }
    public Item item;

    void Start() {

    }

    public void SetRideItem(Item item,UIRide owner,bool equiped) {
        this.item = item;
        StringBuilder sb = new StringBuilder();
        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) {
            sb.Append("Lv.");
            sb.Append(this.item.Define.Level.ToString());
            this.level.text = sb.ToString();
        } 
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }
}
