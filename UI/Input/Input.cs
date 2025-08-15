using UnityEngine;

class InputBox{
    static Object cacheObject = null;

    //Show会返回一个UIInputBox实例，可能直接Show.onSubmit绑定函数，且InputBox解决方案内部全局可用
    public static UIInputBox Show(string message,string title = "",string buttonOK ="",string buttonCancel = "",string emptyTips = "") {
        if (cacheObject == null) {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go =(GameObject)GameObject.Instantiate(cacheObject);
        UIInputBox inputBox = go.GetComponent<UIInputBox>();
        inputBox.Init(title,message,buttonOK,buttonCancel,emptyTips);
        return inputBox;
    }

}