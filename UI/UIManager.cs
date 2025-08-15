using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {
    
    class UIElement {
		public string Resources;
		public bool Cache;
		public GameObject Instance;
	}

	private Canvas CurrentUIcanvas;


    private Dictionary<Type,UIElement> UIResources = new Dictionary<Type,UIElement>();

    public event Action OnMoneyChanged;
    public event Action OnBagItemChanged;
    //ALL newly added UI prefab will be add in this
    //these path info of Resource is needed when those UI panels are loaded
    public UIManager() {
        this.UIResources.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest",Cache=true} ); 
		this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/Bag/UIBag", Cache = false });
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/Shop/UIShop", Cache = false });
        this.UIResources.Add(typeof(UICharacterEquip), new UIElement() { Resources = "UI/Equip/UICharacterEquip", Cache = false });

        //Quest
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/Quest/UIQuestSystem", Cache = false });
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/Quest/UIQuestDialog", Cache = false });

        //Friend
        this.UIResources.Add(typeof(UIFriends), new UIElement() { Resources = "UI/Friend/UIFriends", Cache = false });

        //Guild
        this.UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/Guild/UIGuild", Cache = false });
        this.UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/Guild/UIGuildList", Cache = false });
        this.UIResources.Add(typeof(UIGuildPopNotifyGuild), new UIElement() { Resources = "UI/Guild/UIGuildPopNotifyGuild", Cache = false });
        this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/Guild/UIGuildPopCreate", Cache = false });
        this.UIResources.Add(typeof(UIGuildApplyList), new UIElement() { Resources = "UI/Guild/UIGuildAppliesList", Cache = false });
        //Setting
        this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetting", Cache = false });
        //PopCharacterMenu
        this.UIResources.Add(typeof(UIPopCharacterMenu), new UIElement() { Resources = "UI/UIPopCharacterMenu", Cache = false });
        //Ride
        this.UIResources.Add(typeof(UIRide), new UIElement() { Resources = "UI/Ride/UIRide", Cache = false });
        //SystemConfig
        this.UIResources.Add(typeof(UISystemConfig), new UIElement() { Resources = "UI/UISystemConfig", Cache = false });
    }

    ~UIManager() {

	}

	public void UpdateMoney() {
        
        OnMoneyChanged?.Invoke();
        Debug.Log("OnMoneyChanged 已触发");
    }

	public void UpdateBagItem() {
		if(OnBagItemChanged!=null)
		OnBagItemChanged.Invoke();

        Debug.Log("UpdateBagItem 已触发,自动整理背包");
    }

   
	public T Show<T>() {
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);

		Type type = typeof(T);
		if (this.UIResources.ContainsKey(type)) {

			UIElement UIElemntInfo = this.UIResources[type];
			if(UIElemntInfo.Instance != null) {
                //UIElemntInfo.Instance.SetActive(true);
                CurrentUIcanvas = UIElemntInfo.Instance.GetComponent<Canvas>();
                CurrentUIcanvas.enabled = true;
                //UIElemntInfo.Instance.gameObject.transform.localScale = Vector3.one;
				Debug.Log(UIElemntInfo.Instance.gameObject.name);
            } else {
				UnityEngine.Object prefab = Resources.Load(UIElemntInfo.Resources);
                if (prefab == null) {
					return default(T);
				}
                UIElemntInfo.Instance = (GameObject)GameObject.Instantiate(prefab);
                CurrentUIcanvas = UIElemntInfo.Instance.GetComponent<Canvas>();// 如果在该资源的预制体不存在，则返回值为Null
                //this line instantiate the UI panel with prefab name 
                //UIElemntInfo.Instance = (GameObject)GameObject.Instantiate(prefab,UIMain.Instance.transform);
            }
			return UIElemntInfo.Instance.GetComponent<T>();
		}
		return default(T);
	}

	public void Close(Type type) {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        if (this.UIResources.ContainsKey(type)) {
			UIElement UIElemntInfo = this.UIResources[(type)];
			if (UIElemntInfo.Cache) {
                //info.Instance.SetActive(false);
                //开销大
                // 获取实例化后的Canvas组件并禁用

                CurrentUIcanvas = UIElemntInfo.Instance.GetComponent<Canvas>();
                CurrentUIcanvas.enabled = false; // 停止渲染但保留布局计算

                Debug.Log(UIElemntInfo.Instance.gameObject.name);
                //scale变为0
            } else {
				GameObject.Destroy(UIElemntInfo.Instance);
				UIElemntInfo.Instance = null;
			}
		}
	}

    public void Close<T>() {
        this.Close(typeof(T));
    }

}
