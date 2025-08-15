using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour {

    public Collider minimapBoundingBox;
    public Image minimap;
    public Image arrow;
    public Text mapName;    

    private Transform playerTransform;

	void Start () {
        MinimapManager.Instance.minimap = this;
        this.UpdateMap();
    }

    public void UpdateMap() {
        this.mapName.text = User.Instance.CurrentMapData.Name;
        //if (this.minimap.overrideSprite == null) 每次都必须 更新
        this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();

        this.minimap.SetNativeSize();//手动设置原始大小
        this.minimap.transform.localPosition = Vector3.zero;

         this.minimapBoundingBox = MinimapManager.Instance.MinimapBoundingBox;
         this.playerTransform = null;

        //this.playerTransform = User.Instance.CurrentCharacterObject.transform;
        //换到Update中处理
        }

        void Update () {

        if (playerTransform == null) {
            playerTransform = MinimapManager.Instance.PlayerTransform;
        }
        
        if(minimapBoundingBox == null || playerTransform==null) {
            return;
        }
        float realWidth = minimapBoundingBox.bounds.size.x;
        float realHeight = minimapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;//坐标系不同

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);//利用中心点移动来实现小地图映射世界位置 
        this.minimap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);//使箭头旋转
	}
}
