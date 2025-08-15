using Assets.Scripts.Managers;
using Entities;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject nameBarPrefab;
    public GameObject npcStatusPrefab;

    private Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform,GameObject> elementStatus = new Dictionary<Transform, GameObject>();

	protected override void OnStart () {
		
	}
	void Update () {
		
	}


    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject gameObjectNameBar = Instantiate(nameBarPrefab, this.transform);
        gameObjectNameBar.name = "NameBar" + character.entityId;
        gameObjectNameBar.GetComponent<UIWorldElement>().owner = owner;
        gameObjectNameBar.GetComponent<UINameBar>().character = character;
        gameObjectNameBar.SetActive(true);
        this.elementNames[owner] = gameObjectNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementNames.ContainsKey(owner))
        {
            Destroy(this.elementNames[owner]);
            this.elementNames.Remove(owner);
        }
    }

    public void AddNpcQuestStatus(Transform owner,NpcQuestStatus status) {
        if (this.elementStatus.ContainsKey(owner)) {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        } else {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus"+owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }
    }

    public void RemoveNpcQuestStatus(Transform owner) {
        if (this.elementStatus.ContainsKey(owner)) {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }
    }

}
