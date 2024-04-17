using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGingerMan : EnemyBase
{
    public const int _HP = 50;
    const float RANGE = 10;
    const float CHANGEDISTANCE = 1.75f;
    private Vector3 targetPoint;
    private Vector3 BasePosition;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        BasePosition = this.gameObject.transform.position;
        targetPoint = MovePoint(BasePosition);
    }
    public void Update()
    {
        #region デバッグ用機能
        var position = this.transform.position;
        Vector3 direction = this.transform.forward;
        float distance = 10.0f;
        Color color = Color.red;
        Debug.DrawRay(position, direction * distance, color);

        #endregion
        StateControl();
        switch (enemyState)
        {
            case EnemyState.Stay:
                Stay();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
        if (HP <= 0)
        {
            Death();
        }
        animReset();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Damage(hitDamage);
        }
    }
    public override void Stay()
    {
        if (Vector3.Distance(targetPoint, this.transform.position) <= CHANGEDISTANCE)
        {
            targetPoint = MovePoint(BasePosition);
        }
        Vector3 direction = (targetPoint - this.transform.position).normalized;
        this.transform.rotation = Quaternion.LookRotation(direction);
        rb.AddForce(direction * Speed, ForceMode.Force);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
    private Vector3 MovePoint(Vector3 position)
    {
        float moveX = Random.Range(position.x - RANGE, position.x + RANGE);
        float moveZ = Random.Range(position.z - RANGE, position.z + RANGE);
        return new Vector3(moveX, transform.position.y, moveZ);
    }
}
