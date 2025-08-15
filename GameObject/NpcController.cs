using System.Collections;
using UnityEngine;
using Common.Data;
using Managers;
using Utilities;
using Models;
using Assets.Scripts.Models;
using Assets.Scripts.Managers;

public class NpcController : MonoBehaviour {

	[SerializeField] private int npcId;

	SkinnedMeshRenderer renderer;
    private Animator animator;
    Color originalColor;
    [SerializeField] private Color highlightedColor;

	private bool interactive = false;

	NpcDefine npc;
    NpcQuestStatus questStatus;

    void Start () {
        //renderer = this.gameObject.GetComponent<SkinnedMeshRenderer>();
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        animator = this.gameObject.GetComponent<UnityEngine.Animator>();
		originalColor = renderer.sharedMaterial.color;
		npc = NPCManager.Instance.GetNpcDefine(npcId);
        NPCManager.Instance.UpdateNpcPosition(this.npcId,this.transform.position);
		this.StartCoroutine(Actions());
        RefreshNpcStatus();
        QuestManager.Instance.QuestStatusChanged += OnquestStatusChanged;
    }

    void OnquestStatusChanged(Quest quest) {
        this.RefreshNpcStatus();
    }

    private void RefreshNpcStatus() {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcId);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform,questStatus);
    }

    void OnDestroy() {
        QuestManager.Instance.QuestStatusChanged -=OnquestStatusChanged;
        if (UIWorldElementManager.Instance != null) {
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
        }
    }

    IEnumerator Actions() {
        //random actions,over x seconds,npc will do a relax action
        while (true) {      //inifite loop
            if (interactive) {
                 yield return new WaitForSeconds(2f);
            }else {
				yield return new WaitForSeconds(Random.Range(5f, 10f));
			}
			this.NpcRelax();
		}
	}
    //点击触发交互逻辑的入口
    private void OnMouseDown() {//点击后自动寻路到该npc
        if (Vector3.Distance(this.transform.position, User.Instance.CurrentCharacterObject.transform.position) > 2f) {
            User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
        }
        Interactive();
        Debug.Log("npc: " + this.name);
    }
    private void Interactive() {
        if (!interactive) {//avoid repetive click
            interactive = true;//before  setting it to ture,after setting it to false
            StartCoroutine(DoInterative());//execute interation in Coroutine
        }
    }
    IEnumerator DoInterative() {
		yield return FaceToPlayer();
		if(NPCManager.Instance.Interactive(npc)) {
			NpcTalk();
        }
		yield return new WaitForSeconds(3f);
		interactive = false;//why set interactive to false?
    }
	IEnumerator FaceToPlayer() {
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position-this.transform.position).normalized;
		// the result vector will point to Character
		// thereby we find the direction vector to make npc face to player

		while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo))>5f) {
			this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward,faceTo,Time.deltaTime * 5f);
			yield return null;
		}
		//rotate npc until the angle less 5 degree
	}
    private void OnMouseOver() {
        Highlitht(true);
    }
    private void OnMouseEnter() {
        Highlitht(true);
    }
    private void OnMouseExit() {
        Highlitht(false);
    }
    //悬停高亮
    private void Highlitht(bool isHighlight) {
        if (isHighlight) {
            if (renderer.sharedMaterial.color != highlightedColor) {
                renderer.sharedMaterial.color = highlightedColor;
            }
        } else {
            if (renderer.sharedMaterial.color != originalColor) {
                renderer.sharedMaterial.color = originalColor;
            }
        }
    }
    private void NpcRelax() {
        animator.SetTrigger(AnimatorParameter.Actions.Relax.ToString());
    }
    private void NpcTalk() {
        animator.SetTrigger(AnimatorParameter.Actions.Talk.ToString());
    }
    void Update () {
		
    }
}
