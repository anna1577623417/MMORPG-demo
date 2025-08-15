using Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI textArea;//聊天内容显示区域
    [SerializeField] private TabView channelTab;
    [SerializeField] private InputField chatText;//input component
    [SerializeField] private GameObject chatTargetObject;
    [SerializeField] private Text chatTarget;

    [SerializeField] private Dropdown channelSelect;

    void Start() {
        this.channelTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }
    private void OnDestroy() {
        ChatManager.Instance.OnChat -= RefreshUI;
    }
    void Update() {
        //进入输入时，避免同时控制任务移动
        InputManager.Instance.IsInputMode = chatText.isFocused;
    }
    private void OnDisplayChannelSelected(int inx) {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)inx;
        RefreshUI();
    }

    private void RefreshUI() {
        this.textArea.text = ChatManager.Instance.GetCurrentMessages();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if (ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private) {
            this.chatTargetObject.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0) {
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            } else {
                this.chatTarget.text = "<无>:";
            }

        } else {
            this.chatTargetObject.gameObject.SetActive(false);
        }
    }

    public void OnClickSend() {
        _OnEndInput(this.chatText.text);
    }
    public void _OnEndInput(string text) {
        if(!string.IsNullOrEmpty(text.Trim())) {
            this.SendChat(text);
        }
        this.chatText.text = "";//clean inputarea after sending messages
    }
    private void SendChat(string content) {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }

    public void OnSendChannelChanged() {
        //缺少了一个综合，所以进行偏移1位
        if(ChatManager.Instance.sendChannel ==(ChatManager.LocalChannel)(channelSelect.value+1)) {
            return;
        }
        //如果切换频道失败，如切换成队伍，但没有队伍，则返回原来的频道
        //如果切换成功，则刷新
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)channelSelect.value + 1)){
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
            this.channelSelect.RefreshShownValue();
        } else {
            this.RefreshUI();
        }

    }
}
