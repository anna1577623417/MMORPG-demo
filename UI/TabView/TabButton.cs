using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TabButton :MonoBehaviour {
    [SerializeField]private Sprite activeImage;
    private Sprite normalImage;

    public TabView tabView;

    public int tabIndex;
    public bool selected = false;

    private Image tabImage;

    void Start() {
        tabImage = this.GetComponent<Image>();
        normalImage = tabImage.sprite;

        this.GetComponent<Button>().onClick.AddListener(Onclick);
    }
    public void Select(bool select) {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    void Onclick() {
        this.tabView.SelectTab(this.tabIndex);//when we click this button,it switches to corresponding page
    }
}

