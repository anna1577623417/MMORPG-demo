using System;
using Network;
using UnityEngine;
using Common.Data;
using SkillBridge.Message;
using Models;
using Managers;
namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public int CurrentMapId {  get;  set;  }

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            foreach (var character in response.Characters)
            {
                if (User.Instance.CurrentCharacter==null || (character.Type ==CharacterType.Player
                    &&User.Instance.CurrentCharacter.Id == character.Id  ))
                {
                    User.Instance.CurrentCharacter = character;
                    //更新角色最新信息
                }
                CharacterManager.Instance.AddCharacter(character);
            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);//当前角色切换地图
                this.CurrentMapId = response.mapId;//成功加载后更新当前地图信息
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharacterID : {0}", response.entityId);
            if (response.entityId != User.Instance.CurrentCharacter.Id) {
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            } else {
                CharacterManager.Instance.Clear();
            }
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);//加载目标场景，进入地图
                SoundManager.Instance.PlayMusic(map.Music);
            }
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }
        public void SendMapEntitySync(EntityEvent entityEvent,NEntity entity,int param) {
            //Debug.LogFormat("MapEntityUpdateRequest ：ID：{0}  POS：{1}  DIR：{2}  SPD {3}",entity.Id,entity.Position.String(),entity.Direction.String(),entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync() {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity,
                Param = param,
            }; 
            NetClient.Instance.SendMessage(message);
        }
        //接受同步信息的入口
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response) {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendFormat("MapEntityUpdateResponse：Entitys：{0}",response.entitySyncs.Count);
            stringBuilder.AppendLine();
            foreach(var entity in response.entitySyncs) {
                EntityManager.Instance.OnEntitySync(entity);//实际做同步的代码
                stringBuilder.AppendFormat("Id:[0]  evt：{1}  entity{2}",entity.Id,entity.Entity,entity.Entity.String());
                stringBuilder.AppendLine();
            }
            Debug.Log(stringBuilder.ToString());
        }

        internal void SendMapTeleporter(int teleporterID) {

            Debug.LogFormat("MapTelePorterRequest：teleporterID{0}", teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }
    }
}
