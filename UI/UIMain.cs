using Models;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SceneName;
using Managers;
public class UIMain : MonoSingleton<UIMain> {

	[SerializeField] private Text avatarName;
	[SerializeField] private Text avatarLevel;

	public UITeam TeamWindow;
    protected override void OnStart() {
        this.UpdateAvatar();
	}

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }
	
	void Update () {
		
	}

	public void OnClickTest() {
		UITest test = UIManager.Instance.Show<UITest>();
		test.SetTitle("测试UI");
        test.OnClose += Test_OnClose;
	}

    private void Test_OnClose(UIWindow sender, UIWindow.WindowResult result) {
		(sender as UITest).name = "UITest";//把父类转换成子类
        (sender as UIWindow).name = "UITest";

        MessageBox.Show("点击了对话框的：" + result, "\n对话框响应结果：", MessageBoxType.Information);
    }

	public void OnClickBag() {
		Debug.Log("Clicked the Bag!");
        //UIManager.Instance.Show<UIBag>();
        BagManager.Instance.ShowBag();
    }

	public void OnClickCharacterPanel() {
		UIManager.Instance.Show<UICharacterEquip>();
	}

	public void OnClickQuest() {
		UIManager.Instance.Show<UIQuestSystem>();
	}

    public void OnClickFriends() {
        UIManager.Instance.Show<UIFriends>();
    }

	public void ShowTeamUI(bool show) {
		TeamWindow.ShowTeam(show);
	}

	public void OnClickGuild() {
		GuildManager.Instance.ShowGuild();
	}

	public void OnClickRide() {
		UIManager.Instance.Show<UIRide>();
	}

	public void OnClickSetting() {
		UIManager.Instance.Show<UISetting>();
	}

	public void OnClickSkill() {

	}
}
