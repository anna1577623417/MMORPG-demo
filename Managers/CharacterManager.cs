﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Entities;
using SkillBridge.Message;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();


        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {

        }

        public void Init()
        {

        }

        public void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach(var key in keys) {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        public void AddCharacter(NCharacterInfo NcharacterInfo)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", NcharacterInfo.Id, NcharacterInfo.Name, NcharacterInfo.mapId, NcharacterInfo.Entity.String());
            Character character = new Character(NcharacterInfo);
            this.Characters[character.entityId] = character;
            this.Characters[NcharacterInfo.Id] = character;
            EntityManager.Instance.AddEntity(character);
            if (OnCharacterEnter!=null)
            {
                OnCharacterEnter(character);
                //处理实例对象生成的封装的委托层（该委托在GameObjectManager注册了实际生成游戏对象的函数）
            }
        }


        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);
            if (this.Characters.ContainsKey(entityId)) {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);  
                if (OnCharacterLeave!=null) {
                    OnCharacterLeave(this.Characters[entityId]);
                }
                this.Characters.Remove(entityId);
            }
        }

        public Character GetCharacter(int id) {
            Character character;
            this.Characters.TryGetValue(id, out character);
            return character;
        }
    }
}
