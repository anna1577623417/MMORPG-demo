﻿using SkillBridge.Message;
using System.Collections.Generic;

namespace Managers {
    public class FriendManager : Singleton<FriendManager> {
        public List <NFriendInfo> allFriends;
        public void Init(List<NFriendInfo> friends) {
            this.allFriends = friends;
        }
    }
}

