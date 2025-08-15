﻿using UnityEngine;
using Entities;
using SkillBridge.Message;
using Services;
using Managers;
using UnityEngine.AI;
using System.Collections;


public class PlayerInputController : MonoBehaviour {

    public Rigidbody rb;
    CharacterState state;

    public Character character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    private NavMeshAgent navMeshAgent;

    private bool autoNav = false;

    // Use this for initialization
    void Start () {
        state =CharacterState.Idle;

        if(navMeshAgent == null ) { 
            navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = 0.3f;
        }
    }

    public void StartNav(Vector3 target) {
        StartCoroutine(BeginNav(target));
    }
    IEnumerator BeginNav(Vector3 target) {
        navMeshAgent.SetDestination(target);
        yield return new WaitForSeconds(rotateSpeed);
        autoNav = true;
        if(state !=CharacterState.Move) {
            state = CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            navMeshAgent.speed = this.character.speed / 100f;
        }
    }

    public void StopNav() {
        autoNav = false;
        navMeshAgent.ResetPath();

        if(state != CharacterState.Idle) {
            state = CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRenderer.Instance.SetPath(null, Vector3.zero);
    }

    public void NavMove() {
        if (navMeshAgent.pathPending) return;
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid) {
            StopNav();
            return;
        }
        if (navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete) return;

        if(Mathf.Abs(Input.GetAxis("Vertical"))>0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1) {
            StopNav();
            return ;
        }
        NavPathRenderer.Instance.SetPath(navMeshAgent.path, navMeshAgent.destination);//有路径有目标点 
        if(navMeshAgent.isStopped || navMeshAgent.remainingDistance < 0.3f) {
            StopNav();
            return;
        }
    }
    void FixedUpdate()
    {
        if (character == null) return;

        if (InputManager.Instance!=null&& InputManager.Instance.IsInputMode) return;

        if (autoNav) {
            NavMove();
            return;
        }
        
        float v = Input.GetAxis("Vertical");//W和S
        if (v > 0.01)//move forward
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else if (v < -0.01)//move back
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else//-0.1<=v<=0.1,认为玩家不同
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero; //刚体速度置0
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }

        //没有完善跳跃键，只是播放动画
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if (h<-0.1 || h>0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);//直接控制所挂载的对象的Transform进行转向
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);
            
            if(rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }

        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }
    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        if (this.character == null) {
            Debug.LogError(" NullExpected :PlayerInputController : "+this.character );
            return;
        }
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
        this.lastPos = this.rb.transform.position;

        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 10)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));//控制移动的代码
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir,this.transform.forward);

        if(rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle)) {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));////控制转向的代码
            this.SendEntityEvent(EntityEvent.None);
        }

    }
     
    public void SendEntityEvent(EntityEvent entityEvent,int param=0)
    {
        if (entityController != null) {
            entityController.OnEntityEvent(entityEvent,param);
        }
        MapService.Instance.SendMapEntitySync(entityEvent,this.character.EntityData,param);
    }
}
