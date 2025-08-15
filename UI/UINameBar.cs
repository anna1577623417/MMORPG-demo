using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExcuteInEditMode]
public class UINameBar : MonoBehaviour {

    [SerializeField] private Image avatar;
    [SerializeField]private Text characterName;
    
    public Character character;


    void Start () {
		if(this.character!=null)
        {

        }
	}
	void Update () {
        this.UpdateInfo();
	}

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if(name != this.characterName.text)//防止重绘，提高性能
            {
                this.characterName.text = name;
            }
        }
    }
}
