using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharacterMenu : UIWindow, IDeselectHandler {

    public int targetId;
    public string targetName;

    //任意点其他地方(窗口意外的地方),触发该函数,用来关闭UI
    public void OnDeselect(BaseEventData eventData) {
        var ed = eventData as PointerEventData;//转换成子类获取有效信息
        if (ed.hovered.Contains(this.gameObject)) return;//保证点击的地方是UI以外的地方
        this.Close(WindowResult.None);
    }

    //更方便地实现，点击其他任意地方关闭UI
    public void OnEnable() {
        this.GetComponent<Selectable>().Select();//强制启用选择
        this.Root.transform.position = Input.mousePosition + new Vector3 (80, 0, 0);//再点击的地方偏移80个单位再显示UI
    }

    public void OnChat() {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(WindowResult.None);
    }


    //扩展作业
    public void OnAddFriend() {
        this.Close(WindowResult.No);
    }
    public void OnInviteTeam() {
        this.Close(WindowResult.No);
    }
}

