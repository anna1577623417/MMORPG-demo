using UnityEngine;
using System;
public abstract class UIWindow : MonoBehaviour {
	public delegate void CloseHnadler(UIWindow sender , WindowResult result);

	public event CloseHnadler OnClose;

	public GameObject Root;

	public virtual Type Type {  get { return this.GetType(); } }	


	public enum WindowResult {
		None = 0,
		Yes,
		No,
	}
	
	public void Close(WindowResult result = WindowResult.None) {
		UIManager.Instance.Close(this.Type);
		if(this.OnClose != null) {
			this.OnClose(this, result);//若不为空，触发关闭事件
		}
		this.OnClose = null;
		//when UI panel is closed,you can not click to call this method agein
		//everytime users open a UI panel,Onclose will be assigned 
	}
	 
	public virtual void OnCloseClick() {
		this.Close();	
	}

	public virtual void OnyesClick() {
		this.Close(WindowResult.Yes);
	}

	public virtual void OnNoClick() {
        this.Close(WindowResult.No);
    }
	void OnMouseDown() {
		Debug.LogFormat(this.name + "Clicked");
	}


}
