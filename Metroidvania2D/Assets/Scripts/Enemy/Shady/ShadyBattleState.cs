using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadyBattleState : EnemyState
{
    private Transform player;
    private Enemy_Shady enemy;
    private int moveDir;

    private float defultSpeed;
    public ShadyBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    
    public override void Enter()
    {
        base.Enter();

        defultSpeed = enemy.moveSpeed;

        enemy.moveSpeed = enemy.battleStatemoveSpeed;
        
        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                enemy.stats.KillEntity();
                //stateMachine.ChangeState(enemy.deadState);
            }
        }
        else 
        {
            if (stateTimer < 0||Vector2.Distance(player.transform.position,enemy.transform.position)>7)
                stateMachine.ChangeState(enemy.idleState);
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    private void BattleStateFlipControl()
    {
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x&&enemy.facingDir==1)
            enemy.Flip();
    }

    public override void Exit()
    {
        base.Exit();
        
        enemy.moveSpeed=defultSpeed;
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minattackCooldown, enemy.maxattackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }
    
}
