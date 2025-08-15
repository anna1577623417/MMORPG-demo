using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	[SerializeField] private Collider miniMapBoundingbox;
    //use this class to check  if the map has been changed
    //according this to update minimap
    //used to update minimap when players enter another map
    void Start () {
		MinimapManager.Instance.UpdateMinimap(miniMapBoundingbox);
	}

	void Update () {
		
	}
}
