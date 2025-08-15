using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerDownHandler {

    public Image icon;
    public Text title;
    public Text level;
    public Text LimitClass;
    public Text limitCatergory;

    public Image background;
    public Sprite normalBackground;
    public Sprite hightlightBackground;

    [SerializeField]private Selectable Selectable;

    private bool selected;
    public bool Selected {
        get { return selected; }
        set {
            if (selected == value) return;
            selected = value;
            this.background.overrideSprite = selected ? hightlightBackground : normalBackground;}
    }

    public int index { get;set; }
    private UICharacterEquip owner;//put equip item in a owner(UICharacterEquip)

    public Item item;

    void Start () {

}

    bool isEquiped = false;

    //use bool equiped to identity which list( equip item on or off character)
    internal void SetEquipItem(int index, Item item, UICharacterEquip owner, bool equiped) {
        this.owner = owner;
        this.index = index;
        this.item = item;
        this.isEquiped = equiped;

        if (this.title != null) this.title.text = this.item.Define.Name;
        if(this.level !=null) this.level.text = "lv."+item.Define.Level.ToString();
        if(this.LimitClass !=null) this.LimitClass.text = item.Define.LimitClass.ToString();
        if (this.limitCatergory != null) this.limitCatergory.text = item.Define.Category;
        if(this.icon !=null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }
    
    //右键取消选择状态，左键点一次选择，左键点两次穿戴
    public void OnPointerDown(PointerEventData eventData) {
        if (this.isEquiped) {
            UnEquip();
        } else {
           //cancel hightlighten state
            if (this.selected&&eventData.button== PointerEventData.InputButton.Right) {
                this.Selected = false;
                Selectable.interactable = true;
                EquipManager.Instance.PrevSelectedEquip = null;
            } else if(this.selected && eventData.button == PointerEventData.InputButton.Left) {
                //double click logic 
                //2nd click and set selected state to false
                DoEquip();
                this.Selected = false;
                EquipManager.Instance.PrevSelectedEquip = null;
            } else if(!this.selected&&eventData.button == PointerEventData.InputButton.Left) {
                this.Selected = true; //1st click,we have to use Selected(setter) but selected,
                //otherwise,we will trigger the selected highlight  
                EquipManager.Instance.HightlightSwitch(this);
            }

        }
    }

    private void DoEquip() {
        if (this.item.Define.LimitClass != User.Instance.CurrentCharacter.Class) {
            MessageBox.Show(string.Format("无法装备：[{0}]\n职业错误！ ", this.item.Define.Name), "确认", MessageBoxType.Confirm);
            return;
        }
        var msg = MessageBox.Show(string.Format("要装备[{0}]吗? ",this.item.Define.Name),"确认",MessageBoxType.Confirm);
        msg.OnYes = () => {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null) {
                var newmsg = MessageBox.Show(string.Format("要替换掉[{0}]吗", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                newmsg.OnYes = () => {
                    this.owner.DoEquip(this.item);
                };
            } else {
                this.owner.DoEquip(this.item);
            }
        };
    }

    private void UnEquip() {
        if (this.item.Define.LimitClass != User.Instance.CurrentCharacter.Class) {
            MessageBox.Show(string.Format("卸下了错误装备：[{0}]\n职业错误！", this.item.Define.Name), "确认", MessageBoxType.Confirm);
            return;
        }
        var msg = MessageBox.Show(string.Format("要取下装备 [{0}] 吗? ", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () => {
            this.owner.UnEquip(this.item);
        };
    }

    
}
