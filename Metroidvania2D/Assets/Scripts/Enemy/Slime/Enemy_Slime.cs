using UnityEngine;

public enum SlimeType
{
    big,
    medium,
    small
}
public class Enemy_Slime : Enemy
{
    [Header("Slime Specific")] 
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int slimeToCreate;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField]private Vector2 maxCreationVelocity;

    public SlimeIdleState idleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeDeadState deadState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDir(-1);

        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        deadState = new SlimeDeadState(this, stateMachine, "Idle", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);

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
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);

        if (slimeType == SlimeType.small)
        {
            return;
        }
        CreateSlimes(slimeToCreate,slimePrefab);
    }

    private void CreateSlimes(int _amoutOfSlimes, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amoutOfSlimes; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);
            newSlime.GetComponent<Enemy_Slime>().SetupSlime(facingDir);
        }
    }

    public void SetupSlime(int _facingDir)
    {
        if (_facingDir != facingDir)
            Flip();
        
        float xVelocity=Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
        float yVelocity=Random.Range(minCreationVelocity.y, maxCreationVelocity.y);

        isKnocked = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity*facingDir, yVelocity);
        Invoke("CancelKnockback",1.5f);
    }
    private void CancelKnockback()=>isKnocked = false;
}
