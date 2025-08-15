using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIWindow {
	public string name;
	public Text title;

	public void SetTitle(string title) {
		this.title.text = title;
	}
}
