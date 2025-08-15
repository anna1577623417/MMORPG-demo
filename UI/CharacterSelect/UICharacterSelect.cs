using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
public class UICharacterSelect : MonoBehaviour {

    public GameObject panelCreate;
    public GameObject panelSelect;

    public GameObject btnCreateCancel;

    public InputField charName;
    CharacterClass charClass;

    public Transform uiCharListParent;
    public GameObject uiCharInfo;

    public List<GameObject> uiCharactersList = new List<GameObject>();

    public Image[] titles;

    public Text descs;

    public Text[] names;

    private int selectCharacterIdx = -1;

    private bool ReadyToClick=false;

    public UICharacterView characterView;


    // Use this for initialization
    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
        //委托名和监听函数名可以相同
        SceneManager.Instance.ReadyToClickPlayButtonAgain += SwitchClickedState;
    }


    public void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);

        if (init)
        {
            foreach (var old in uiCharactersList)
            {
                Destroy(old);
            }
            uiCharactersList.Clear();

            for(int i=0;i<User.Instance.Info.Player.Characters.Count;i++)
            {

                GameObject go = Instantiate(uiCharInfo, this.uiCharListParent);
                UICharInfo chrInfo = go.GetComponent<UICharInfo>();
                chrInfo.info = User.Instance.Info.Player.Characters[i];

                Button button = go.GetComponent<Button>();
                int idx =i;
                button.onClick.AddListener(() => {
                    OnSelectCharacter(idx);
                });
                //滚动条按键功能监听
                //这里的i是预选角色列表的索引

                uiCharactersList.Add(go);
                go.SetActive(true);
            }
        }
        if (User.Instance.Info.Player.Characters.Count > 0) {
            OnSelectCharacter(0);//默认选择第一位角色
        } else {
            //提示创建新角色
        }
    }

    public void InitCharacterCreate()
    {
        panelCreate.SetActive(true);
        panelSelect.SetActive(false);
        OnSelectClass(1);//创建角色时默认会选择第一个
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
    }


    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;//编辑器传入的charClass和协议中第一个职业都是1值开始

        characterView.CurrectCharacter = charClass - 1;//因此需要转码使得其符合数组的0，1，2，3索引

        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass-1);
            names[i].text = DataManager.Instance.Characters[i+1].Name;
        }

        descs.text = DataManager.Instance.Characters[charClass].Description;

    }


    private void OnCharacterCreate (Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);

        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    public void OnSelectCharacter(int index)
    {
        this.selectCharacterIdx = index;
        var character = User.Instance.Info.Player.Characters[index];
        characterView.CurrectCharacter = (int)character.Class-1 ;
        
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo characterImage = this.uiCharactersList[i].GetComponent<UICharInfo>();
            characterImage.Selected = index == i;
            characterImage.hightLight.gameObject.SetActive(characterImage.Selected);
            //遍历角色列表，将选中的那个角色置为true，其他的为false
            //选中高亮的同时把之前的高亮取消
            //通过idx == i来作为判定依据
        }
    }
    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0 && ReadyToClick)
        {
            SwitchClickedState();
            UserService.Instance.SendGameEnter (selectCharacterIdx);
        }
    }

    private void SwitchClickedState() {
        ReadyToClick = ! ReadyToClick;
        Debug.Log("Have Clicked the EnterButton! IsClicked = :"+ReadyToClick);
    }
}
