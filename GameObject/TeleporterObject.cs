using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour {

	public int ID;

	Mesh mesh=null;

	void Start () {
		this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }


#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = Color.green;

		if(this.mesh != null ) {
			Gizmos.DrawWireMesh(this.mesh,this.transform.position + Vector3.up* this.transform.localScale.y * 0.5f, this.transform.rotation,this.transform.localScale);
		}
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);

	}
#endif

	void OnTriggerEnter(Collider other) {
		PlayerInputController playerInputController = other.GetComponent<PlayerInputController>();
		if(playerInputController!=null&& playerInputController.isActiveAndEnabled) {

			TeleporterDefine teleporterDefine = DataManager.Instance.Teleporters[this.ID];

			if(teleporterDefine == null ) {
				Debug.LogErrorFormat("TelePointerObject : [{0}] Enter Telepointer [{1}] , But TelepointerDefine not existed", playerInputController.character.Info.Name, this.ID);
				return;
			}

			Debug.LogFormat("TelePointerObject：Character [{0}] Enter TelePointer [{1}：{2}]", playerInputController.character.Info.Name, teleporterDefine.Name,teleporterDefine.ID);

			if(teleporterDefine.LinkTo>0 ) {//另一端存在传送点（可传送）
				if(DataManager.Instance.Teleporters.ContainsKey(teleporterDefine.LinkTo)) {//配置表存在该传送点
				MapService.Instance.SendMapTeleporter(this.ID);
				} else {
                    Debug.LogErrorFormat("Telepointer ID：{0} Link {1} error !", teleporterDefine.ID, teleporterDefine.LinkTo);
                }
			}
		}
	}
}
