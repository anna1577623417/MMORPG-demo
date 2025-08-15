using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;
using SceneName;

public class UILogin : MonoBehaviour {


    public InputField username;
    public InputField password;
    public Button buttonLogin;
    public Button buttonRegister;

    void Start () {
        UserService.Instance.OnLogin = OnLogin;
    }

    public void OnClickLogin()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }

        // Enter Game
        UserService.Instance.SendLogin(this.username.text,this.password.text);

    }

    void OnLogin(Result result, string message)
    {
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            SceneManager.Instance.LoadScene(Scene.Name.CharacterSelect.ToString());
            //这个Mono单例需要挂载到一个游戏对象上
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }
}
