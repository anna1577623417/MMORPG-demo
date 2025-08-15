using Assets.Scripts.Services;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour {
	 
	[SerializeField]private Text teamTitle;
	public UITeamItem[] Members;
	public ListView list;
	private const int MaxMembers = 5;
	void Start () {
		if(User.Instance.TeamInfo == null) {
			this.gameObject.SetActive(false);//没有队伍则关闭当前队伍项
			return;
		}
		foreach(var item in Members) {//初始化队伍列表框
			this.list.AddItem(item);
		}
	}

	void OnEnable() {
		UpdateTeamUI();
	}

	public void ShowTeam(bool show) {
		this.gameObject.SetActive(show);
		if (show) {
			UpdateTeamUI();
		}
	}

    private void UpdateTeamUI() {
		if (User.Instance.TeamInfo == null) return;
		this.teamTitle.text = string.Format("我的队伍({0}/{1})", User.Instance.TeamInfo.Members.Count, MaxMembers);//动态修改数字并保持1/5这样的格式

		for(int i = 0;i< MaxMembers; i++) {
			if (i < User.Instance.TeamInfo.Members.Count) {
				this.Members[i].SetMemberInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
				this.Members[i].gameObject.SetActive(true);
			} else {
				this.Members[i].gameObject.SetActive(false) ;
			}
		}
    }

	public void OnClickLeave() {
		MessageBox.Show("确定要离开队伍吗？", "退出队伍", MessageBoxType.Confirm, "确定离开", "取消").OnYes = () => {
			TeamService.Instance.SendTeamLeaveRequest(User.Instance.TeamInfo.Id);
		};
	}
}
