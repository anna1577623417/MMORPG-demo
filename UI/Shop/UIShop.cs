using Assets.Scripts.UI;
using Common.Data;
using Managers;
using Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow, IMoneyUpdate {

	public Text title;
	public Text moneyText;
    private UIShopItem selectedItem = null;
    public GameObject shopItem;
	ShopDefine shop;
	public Transform[] itemRoot;//the content pending to add shopitem

	void Start() {
        UIManager.Instance.OnMoneyChanged += UpdateMoney;
		StartCoroutine(InitItems());
	}

	IEnumerator InitItems() {
		int count = 0;
		int page = 0;

		foreach (var kv in DataManager.Instance.ShopItems[shop.ID]) {
			if (kv.Value.Status > 0) {
				GameObject go = Instantiate(shopItem, itemRoot[page]);//this field generates  shopItems
				UIShopItem ui = go.GetComponent<UIShopItem>();
				ui.SetShopItem(kv.Key, kv.Value, this);
				//when the shopItem is created,the function SetShopItem of UIShopItem will initialize all info it  have to 
				count++;
				if(count>= 10) {
					count = 0;
					page++;
					itemRoot[page].gameObject.SetActive(true);	
				}
			}
		}
		yield return null;
	}

	//update the Shop info
	public void SetShop(ShopDefine shop) {
		this.shop = shop;
		this.title.text = shop.Name;
		this.moneyText.text = User.Instance.CurrentCharacter.Gold.ToString();
	}

	
	public void SelectShopItem(UIShopItem item) {
		if (selectedItem!=null && selectedItem == item) {
			//click the same item two times,it cancel the selected status.
			//we set selectedItem back to null
			selectedItem = null;
            //item.Selected = false;//this var has been handled in UIShopItem script
            Debug.Log(" selectedItem : null");
			return;
		}
		//below handling case that user click  two different items
		if (selectedItem != null) {
			selectedItem.Selected = false;//set that former selected item to false,or set Selected to false status	
		}
		selectedItem = item;//to get which shopItem user has selected
		Debug.Log("name : "+selectedItem.title.text+ " ShopItemID : " + selectedItem.ShopItemID);
	}

	public void OnClickBuy() {
		Debug.Log("OnClickBuy");
		if (this.selectedItem == null) {
			MessageBox.Show("请选择购买的道具", "购买提示");
			return;
		}
		//BuyItem method has already been executed within the condition statement.
		//This block is intentionally left blank for now, but we may later add feedback handling, such as error processing.
		//in BuyItem method of ShopManager ,we indrectly send a purchase request to server
		if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID)) {

		}
	}

	public void UpdateMoney() {
		if (this.moneyText != null) {
			this.moneyText.text = User.Instance.CurrentCharacter.Gold.ToString();
		}
		Debug.Log("商店中 UpdateMoney 已执行");
	}
    //unsubscribe when destroyed
    //avoid Event Reference Retention Leading to Memory Leaks
    private void OnDestroy() {
        UIManager.Instance.OnMoneyChanged -= UpdateMoney;
	}
}
