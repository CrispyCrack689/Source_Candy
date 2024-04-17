using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyWatapachi : EnemyBase
{
    public float explosuinTimer;
    private float timerKeep;
    private void Start()
    {
        timerKeep = explosuinTimer;
    }
    private async void Update()
    {
        StateControl();
        switch (enemyState)
        {
            case EnemyState.Stay:
                Stay();
                TimerReset();
                break;
            case EnemyState.Move:
                Move();
                TimerReset();
                break;
            case EnemyState.Attack:
                await UniTask.Delay(TimeSpan.FromSeconds(attackDelay));
                if (this != null)
                {
                    Attack();
                }
                break;
        }
        if (HP <= 0)
        {
            Death();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Damage(hitDamage);
        }
    }

    public override void Attack()
    {
        explosuinTimer -= Time.deltaTime;
        if (explosuinTimer <= 0)
        {
            float explosion = Random.Range(0.5f, 2.5f);
            playerController.Damage(ATK * explosion);
            enemyState = EnemyState.Stay;
            Destroy(this.gameObject);
        }
    }
    private void TimerReset()
    {
        if (explosuinTimer != timerKeep)
        {
            explosuinTimer = timerKeep;
        }
    }
}
