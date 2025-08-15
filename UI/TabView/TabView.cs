using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TabView:MonoBehaviour {
    [SerializeField]private TabButton[] tabButtons;
    [SerializeField] private GameObject[] tabPages;

    [SerializeField] private int index=-1;

    public Action<int> OnTabSelect { get; internal set; }

    //use this for initialization
    IEnumerator Start () {
        for (int i = 0; i < tabButtons.Length; i++) {
            tabButtons[i].tabView = this;//populate page index and tabView(object)
            tabButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();//wait for one frame
        SelectTab(0);//select page 1 as defaulty
    }

    public void SelectTab(int index) {
        if (this.index !=index) {
            for (int i =0; i<tabButtons.Length;i++) {
                tabButtons[i].Select(i == index);//set that tab button to active state,concurrently set others deactive state
                if (i < tabPages.Length) {//tabPages.Length=0,或者i超出了范围，则不会需要切界面
                    tabPages[i].SetActive(i == index);//it is similar with previous field
                }
            }
            if(OnTabSelect !=null) OnTabSelect(index);
        }
    }
}

