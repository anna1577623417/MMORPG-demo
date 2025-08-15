using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour {

	public Image icon;
	public Text title;
	public Text price;
	public Text count;

	public Image background;
	public Sprite normalBackGround;
	public Sprite selectedBackGround;

	private bool selected=false;
	public bool Selected {
		get { return selected; }
		set {
			selected = value;
			this.background.overrideSprite = selected?selectedBackGround:normalBackGround;
			//this line function in changing the BG according selecting status
        }
	}

	public int ShopItemID { get; set; }
	private UIShop shop;

	private ItemDefine item;
	private ShopItemDefine ShopItem { get; set; }


    void Start () {
		
	}
	
	public void SetShopItem(int id,ShopItemDefine shopItem,UIShop owner) {
		this.shop = owner;
		this.ShopItemID = id;
		this.ShopItem = shopItem;
		this.item = DataManager.Instance.Items[this.ShopItem.ItemID];
        //get resource path from DataManager
        //data in Items come from this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);
        this.title.text = this.item.Name;
		this.count.text = ShopItem.Count.ToString();
		this.price.text = ShopItem.Price.ToString();
		this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);//change icon of shopItem according to 

	}

	public void OnItemSelected() {
		Debug.Log("OnItemSelected");
		this.Selected = !this.Selected;		this.shop.SelectShopItem(this);
	}
}
