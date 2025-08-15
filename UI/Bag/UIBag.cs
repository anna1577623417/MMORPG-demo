using Assets.Scripts.UI;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow, IMoneyUpdate {

	public Text moneyText;

    [SerializeField] private Scrollbar scrollbar1;
	[SerializeField] private Scrollbar scrollbar2;
	[SerializeField] private Scrollbar scrollbar3;
    //A static variable belongs to the class, not an instance of the class
    //thereby, we can use it to track a gameObject
    private static float scrollbarvalue1=1;
	private static float scrollbarvalue2=1;
	private static float scrollbarvalue3=1;

    [SerializeField] private Transform[] pages;
    [SerializeField] private GameObject bagItem;

	List<Image> Cells;


    void Start () {
        UIManager.Instance.OnMoneyChanged += UpdateMoney;
        UIManager.Instance.OnBagItemChanged += OnBagItemChanged;
        if (Cells == null) {
			Cells = new List<Image>();
			for(int page = 0; page < pages.Length; page++) {
				Cells.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
			}
		}
		Debug.Log("Cells.count: "+Cells.Count);
		StartCoroutine(InitBages());
	}
	//check again
	IEnumerator InitBages() {

		for (int i = 0; i < BagManager.Instance.Items.Length; i++) {
			var item = BagManager.Instance.Items[i];
			if (item.ItemId > 0) {
				GameObject go = Instantiate(bagItem, Cells[i].transform);//generate and set item bagitem in the cell
				var ui = go.GetComponent<UIIconItem>();
				var def = ItemManager.Instance.Items[item.ItemId].Define;
				ui.SetMainIcon(def.Icon,item.Count.ToString());//set the image and count of bagitem
			}
		}

		for(int i = BagManager.Instance.Items.Length; i < Cells.Count; i++) {
			Cells[i].color = Color.gray;//cells without uncloking,we set its color to gray(details in BagManager)
		}
        Debug.Log("Manager items length" + BagManager.Instance.Items.Length);
        Debug.Log("Manager items" + BagManager.Instance.Items);
        yield return null;
	}
    


    //call this method when bag is created when recreated
    public void SetBagView() {
			scrollbar1.value = scrollbarvalue1;
			scrollbar2.value = scrollbarvalue2;
			scrollbar3.value = scrollbarvalue3;
    }

	//call this method when bag is closed 
	public void cacheBagView() {
		scrollbarvalue1 = scrollbar1.value;
		scrollbarvalue2 = scrollbar2.value;
		scrollbarvalue3 = scrollbar3.value;
    }

    //public void SetTile(string title) {
    //	this.moneyText.text = User.Instance.CurrentCharacter.Id.ToString();
    //}


    public void OnBagItemChanged() {
        OnReset();      // 先整理背包数据
        RefreshBag();   // 再刷新 UI
    }
    //整理背包
    public void OnBagItemsSort() {
        BagManager.Instance.Reset();
        this.Clear();
        RefreshBag();
        //StartCoroutine(InitBages());
    }
    public void OnReset() {
        BagManager.Instance.Reset();
        //to do : initialize and refresh the bag items in cells
        //make objectspool and manage the items 
    }

    //仿照BagManager初始化函数代码做一个刷新
    public void RefreshBag() {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++) { // 遍历struct array背包，更新 UI
            var item = BagManager.Instance.Items[i];
            var ui = Cells[i].transform.GetComponentInChildren<UIIconItem>();

            if (item.ItemId > 0) // 该格子有物品
            {
                var def = ItemManager.Instance.Items[item.ItemId].Define;

                // 如果当前格子没有 UI 物品，创建一个新的 UI
                if (ui == null) {
                    GameObject go = Instantiate(bagItem, Cells[i].transform);
                    ui = go.GetComponent<UIIconItem>();
                }

                // 更新 UI 显示的图标和数量
                if (ui != null) {
                    ui.SetMainIcon(def.Icon, item.Count.ToString());
                }
            } else // 物品为空，销毁 UI
              {
                if (ui != null) {
                    Destroy(ui.gameObject);
                }
            }
        }
    }

    //clear all items in the bag
    private void Clear() {
        for(int i = 0; i < Cells.Count; i++) {
            if (Cells[i].transform.childCount > 0) {
                Destroy(Cells[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void UpdateMoney() {
        if (this.moneyText != null) {
            this.moneyText.text = User.Instance.CurrentCharacter.Gold.ToString();
        }
        Debug.Log("背包中 UpdateMoney 已执行");
    }
    private void OnDestroy() {
        UIManager.Instance.OnMoneyChanged -= UpdateMoney;
        UIManager.Instance.OnBagItemChanged -= OnBagItemChanged;
    }
}
