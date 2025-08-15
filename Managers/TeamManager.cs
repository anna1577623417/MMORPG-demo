using Models;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Managers{
    public class TeamManager : Singleton<TeamManager> {
        public void Init() {

        }
        internal void UpdateTeamInfo(NTeamInfo team) {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        private void ShowTeamUI(bool show) {
            if(UIMain.Instance != null) {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }

}
