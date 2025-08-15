using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour {

   public Transform owner;

    [SerializeField] private float height = 1.5f;

	void Start () {
		
	}
    void Update()
    {
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * height;//名字图标始终跟随主角/怪物
        }
    }
}
