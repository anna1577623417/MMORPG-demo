using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using System;


public class RideController : MonoBehaviour{
    [SerializeField] Transform mountPoint;
    [SerializeField] private  EntityController rider;
    [SerializeField] Vector3 offset;
    private Animator animator;

    void Start() {
        this.animator = this.GetComponent<Animator>();
    }

    //让骑乘者和坐骑保持本地同步
    void Update() {
        if (this.mountPoint == null || this.rider == null) return;

        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    public void SetRider(EntityController rider) {
        this.rider = rider;
    }

    public void OnEntityEvent(EntityEvent entityEvent,int param) {
        switch(entityEvent) {
            case EntityEvent.Idle:
                animator.SetBool("Move", false);
                animator.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                animator.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                animator.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                animator.SetTrigger("Jump");
                break;
        }
    }
}
