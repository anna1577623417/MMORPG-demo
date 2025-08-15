using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour {

    [SerializeField] private Text title;
    [SerializeField] private Text message;
    [SerializeField] private Text tips;
    [SerializeField] private InputField inputField;

    [SerializeField] private Text buttonYesTitle;
    [SerializeField] private Text buttonNoTitle;

    public delegate bool SubmitHandler(string inputText,out string tips);
    public event SubmitHandler onSubmit;
    public UnityAction OnCancel;

    public string emptyTips;
   

    public void Init(string title, string message, string btnOK = "", string btnCancel = "",string emptyTips = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.tips.text = null;
        this.onSubmit = null;//初始化总是置空
        this.emptyTips = emptyTips;

        //文本UI可以通过置空来达到隐藏的效果，无需禁用和激活
        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = title;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = title;
    }

    public void OnClickYes()
    {
        this.tips.text = "";
        if (string.IsNullOrEmpty(inputField.text)) {//若为空则发送提示
            this.tips.text = this.emptyTips;
            return;
        }
        if(onSubmit != null) {
            string tips;
            if(!onSubmit(this.inputField.text,out tips)) {//触发委托的同时返回tips
                this.tips.text = tips;
                return;  
            }
        }
        Destroy(this.gameObject);
        Debug.Log("UIInputBox：Yes");
    }

    public void OnClickNo()
    {
        Destroy(this.gameObject);
        if(this.OnCancel != null) {
            this.OnCancel();
        }
    }
}
