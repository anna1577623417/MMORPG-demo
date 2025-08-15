using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour {

	[SerializeField]private Image mainImage;

    //[SerializeField]private Image secondmage;

    public Text mainText;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void SetMainIcon(string iconName,string text) {
		this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);//use path name to load and sustitute the mainImage
		this.mainText.text = text; //the number text
	}
}
