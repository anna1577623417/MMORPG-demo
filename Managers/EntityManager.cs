using Entities;
using SkillBridge.Message;
using System.Collections.Generic;


namespace Managers {
    interface IEntityNotify {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent entityevent,int param);
    }
    internal class EntityManager: Singleton<EntityManager> {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        Dictionary<int,IEntityNotify> notifiers = new Dictionary<int,IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId,IEntityNotify notify) {
            this.notifiers[entityId] = notify;
        }
        public void AddEntity(Entity entity) {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity) {
            this.entities.Remove(entity.Id);
            if(notifiers.ContainsKey(entity.Id)) {
                notifiers[entity.Id].OnEntityRemoved();//notifiers成员在必须实现OnEntityRemoved，也就是IEntityNotify接口中的函数
                notifiers.Remove(entity.Id);//每次被删除时，遍历每个notifiers成员并执行OnEntityRemoved,以此来实现事件通知的效果
            }
        }

        //并不涉及广播，但是实际更新了entity的数据（来自服务端）,
        internal void OnEntitySync(NEntitySync othersDataSync) {
            Entity localOthersEntity = null;
            entities.TryGetValue(othersDataSync.Id, out localOthersEntity);
                if(localOthersEntity != null) {
                    if(othersDataSync.Entity != null) {
                        localOthersEntity.EntityData = othersDataSync.Entity;//更新来自网络的最新数据
                    }
                    if(notifiers.ContainsKey(othersDataSync.Id)) {//将已经改变的数据进行通知给EntityController
                        notifiers[localOthersEntity.entityId].OnEntityChanged(localOthersEntity);//仅负责打印entity信息
                        notifiers[localOthersEntity.entityId].OnEntityEvent(othersDataSync.Event, othersDataSync.Param);//事件变化
                    }
                }
            }
        
    }
}
