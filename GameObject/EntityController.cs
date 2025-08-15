using SkillBridge.Message;
using UnityEngine;
using Entities;
using Managers;



public class EntityController : MonoBehaviour,IEntityNotify
{

    public Animator animator;
    public Rigidbody rigidbody;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;//在GameObjectManager中初始化，即创建角色时，会绑定entity

    public Vector3 position;
    public Vector3 direction;
    Quaternion rotation;

    public Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    public RideController rideController;
    private int currentRide = 0;
    public Transform rideBone;//a bonding point for sitting

    // Use this for initialization
    void Start () {
        if (entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            this.UpdateTransform();
        }

        if (!this.isPlayer)
            rigidbody.useGravity = false;
    }

    //同步移动时，直接修改其他玩家模型的方法，空间信息由entity提供，而entity又是在创建时绑定的
    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rigidbody.MovePosition(this.position);//移动刚体
        this.transform.forward = this.direction;

        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }
	
    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);

        if(UIWorldElementManager.Instance!=null)//防止重复删除
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    void FixedUpdate()
    {
        if (this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.isPlayer)//其他玩家和NPC
        {
            this.UpdateTransform();
        }
    }

    public void OnEntityEvent(EntityEvent entityEvent,int param)
    {
        switch(entityEvent)
        {
            case EntityEvent.Idle:
                animator.SetTrigger("Idle");
                animator.SetBool("Move", false);
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
            case EntityEvent.Ride:
                this.Ride(param);
                break;
        }
        if(this.rideController !=null) this.rideController.OnEntityEvent(entityEvent,param);//character's ride moves as character does
    }

    public void OnEntityRemoved() {
        if(UIWorldElementManager.Instance != null) {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);//删掉血条
        }
        Destroy(this.gameObject);//删掉自身(不是this.Transform)
    }

    public void OnEntityChanged(Entity entity, int param) {
        Debug.LogFormat("OnEntityChanged ：ID：{0}  POS：{1}  DIR：{2}  SPD {3}", entity.entityId,entity.position,entity,direction,entity.speed);
    }

    public void OnEntityChanged(Entity entity) {
        
    }
    internal void Ride(int rideId) {
        if (currentRide == rideId) return;
        currentRide = rideId;
        if (rideId > 0) {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId,this.transform);//上马
        } else {
            Destroy(this.rideController.gameObject);//下马
            this.rideController = null;
        }

        //using "layer" to manager different state
        if(this.rideController == null) {
            this.animator.transform.localPosition = Vector3.zero;
            this.animator.SetLayerWeight(1,0);
        }else {
            this.rideController.SetRider(this);
            this.animator.SetLayerWeight(1, 1);
        }
    }

    // this.animator.transform.position is player's position
    //始终保持角色和坐骑的相对位置
    public void SetRidePosition(Vector3 position) {
        this.animator.transform.position = position + (this.animator.transform.position - this.rideBone.position);
    }
}
