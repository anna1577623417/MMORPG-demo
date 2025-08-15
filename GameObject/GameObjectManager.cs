using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;
using Models;
using Managers;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> CharactersDictionary = new Dictionary<int, GameObject>();
    // Use this for initialization

    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    void Update()
    {

    }

    void OnCharacterEnter(Character character)
    {
        CreateCharacterObject(character);
    }
    void OnCharacterLeave(Character character) {
        if (!CharactersDictionary.ContainsKey(character. entityId)) {
            return;
        }
        if (CharactersDictionary[character.entityId] != null) {
            Destroy(CharactersDictionary[character.entityId]);
            this.CharactersDictionary.Remove(character.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach (var character in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(character);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character character)
    {
        if (!CharactersDictionary.ContainsKey(character.entityId) || CharactersDictionary[character.entityId] == null)
        {
            Object Object = Resloader.Load<Object>(character.Define.Resource);
            if(Object == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(Object,this.transform);//进入地图时，处理实例化玩家角色和怪物角色对象的语句
            go.name = "Character_" + character.Id + "_" + character.Name;//父物体是GameObjectMananger所挂载的游戏对象

            CharactersDictionary[character.entityId] = go;//写入该角色

            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);//角色名字条
        }
        this.InitGameObject(CharactersDictionary[character.entityId],character);
    }
    private void InitGameObject(GameObject gameObject,Character character) {
        gameObject.transform.position = GameObjectTool.LogicToWorld(character.position);
        gameObject.transform.forward = GameObjectTool.LogicToWorld(character.direction);
        CharactersDictionary[character.Info.Id] = gameObject;

        EntityController entityController = gameObject.GetComponent<EntityController>();
        if(entityController != null) {
            entityController.entity = character;//隐式转换，Character（继承Entity）转换为Entity
            entityController.isPlayer = character.IsCurrentPlayer;//是否是当前玩家（false意味着npc或者其他玩家）
            entityController.Ride(character.Info.Ride);
        }
        PlayerInputController playerInputController = gameObject.GetComponent<PlayerInputController>();

        if (playerInputController != null) {

            if (character.IsCurrentPlayer) {
                User.Instance.CurrentCharacterObject = playerInputController;
                MainPlayerCamera.Instance.player = gameObject;
                playerInputController.enabled = true;
                playerInputController.character = character;
                playerInputController.entityController = entityController; 
            } else {
                playerInputController.enabled = false;
            }

        } 
    }

    public RideController LoadRide(int rideId,Transform parent) {
        var rideDefine = DataManager.Instance.Rides[rideId];
        Object obj = Resloader.Load<Object>(rideDefine.Resource);
        if (obj == null) {
            Debug.LogErrorFormat("Ride{0} Resource[{1} not existed]",rideDefine.ID,rideDefine.Resource);
            return null;
        }
       GameObject go = (GameObject)Instantiate(obj,parent);//实际生成坐骑的代码,父物体是角色
       go.name = "Ride_" + rideDefine.ID + "_" + rideDefine.Name;
        return go.GetComponent<RideController>();
    }
}

