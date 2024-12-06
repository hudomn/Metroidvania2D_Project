using System;
using UnityEngine;

public class Enemy_Archer : Enemy
{
    [Header("Archer specific info")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField]private float arrowSpeed;
    [SerializeField] private int arrowDamage;

    public Vector2 jumpVelocity;
    public float jumpCooldown;
    public float safeDistance;
    [HideInInspector]public float lastTimeJumped;

    [Header("Additional collision checks")]
    [SerializeField]private Transform groundBehindCheck;

    [SerializeField] private Vector2 groundBehindCheckSize;

    
    #region States
    public ArcherIdleState idleState { get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherAttackState attackState { get; private set; }
    public ArcherStunnedState stunnedStae { get; private set; }
    public ArcherDeadState deadState { get; private set; }
    public ArcherJumpState jumpState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new ArcherMoveState(this, stateMachine, "Move", this);
        battleState = new ArcherBattleState(this, stateMachine, "Idle", this);
        attackState = new ArcherAttackState(this, stateMachine, "Attack", this);
        stunnedStae = new ArcherStunnedState(this, stateMachine, "Stunned", this);
        deadState = new ArcherDeadState(this, stateMachine, "Idle", this);
        jumpState=new ArcherJumpState(this, stateMachine, "Jump", this);
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedStae);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);
        
        newArrow.GetComponent<Arrow_Controller>().SetupArrow(arrowSpeed*facingDir,stats);
        
        if(facingDir==-1)
            newArrow.transform.Rotate(0,180,0);
    }

    public bool GroundBehind()=>Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);
    public bool WallBehind()=>Physics2D.Raycast(wallCheck.position,Vector2.right*-facingDir,wallCheckDistance+2,whatIsGround);
    protected void OnDrawGizmosSelected()
    {
        base.OnDrawGizmos();
        
        Gizmos.DrawWireCube(groundBehindCheck.position,groundBehindCheckSize);
    }
}
