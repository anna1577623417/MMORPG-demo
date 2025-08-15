using SceneName;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow {
    public void ExitToCharacterSelect() {
        ////test
        //UIPopCharacterMenu menu = UIManager.Instance.Show<UIPopCharacterMenu>();

        SceneManager.Instance.LoadScene(Scene.Name.CharacterSelect.ToString());
        SoundManager.Instance.PlaySound(SoundDefine.Music_Select);
        UserService.Instance.SendGameLeave();
    }

    public void OnClickSystemConfig() {
        UIManager.Instance.Show<UISystemConfig>();
        this.Close();
    }

    public void ExitGame() {
        UserService.Instance.SendGameLeave(true);
    }
}
