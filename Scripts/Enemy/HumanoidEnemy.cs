using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class HumanoidEnemy : EnemyBase
{
    public const int _HP = 150;
    const float RANGE = 25;
    const float ROTATIONRANGE = 180;
    const float CHANGEDISTANCE = 1.5f;
    [SerializeField]
    float moveDuration = 2f;
    [SerializeField]
    float rotateDuration = 1f;
    private float moveTimer = 0f;
    private float rotateTimer = 0f;
    private float angle = 0;
    private float currentAngle;
    private Vector3 targetPoint;
    private Vector3 basePosition;
    private Vector3 oldPosition;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        basePosition = transform.position;
        targetPoint = MovePoint(basePosition);
        angle = Random.Range(-180f, 180f);
        oldPosition = transform.position;
    }
    private void Update()
    {
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
        animReset();
        if (HP <= 0)
        {
            Death();
        }
    }

    public override void Stay()
    {
        //  索敵行動
        {
            if (moveTimer < moveDuration)
            {
                moveTimer += Time.deltaTime;
                CharMove();
                currentAngle = transform.rotation.y;
            }
            else if (rotateTimer < rotateDuration)
            {
                RotateDirection(angle);
                rotateTimer += Time.deltaTime;
            }
        }

        if (Vector3.Distance(targetPoint, transform.position) <= CHANGEDISTANCE)
        {
            targetPoint = MovePoint(basePosition);
        }
        if (rotateTimer >= rotateDuration)
        {
            angle = Random.Range(-ROTATIONRANGE, ROTATIONRANGE);
            targetPoint = MovePoint(basePosition);
            moveTimer = 0f;
            rotateTimer = 0f;
        }
    }
    /// <summary>
    /// 移動
    /// </summary>
    void CharMove()
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        rb.AddForce(direction * Speed, ForceMode.Force);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
    /// <summary>
    /// 指定した方向に回転
    /// </summary>
    void RotateDirection(float randomAngle)
    {
        //  初期値
        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);
        //  目標角度を作成
        Quaternion targetRotation = Quaternion.Euler(0, randomAngle, 0);
        //  現在の進行度を計算
        var rate = rotateTimer / rotateDuration;
        //  回転処理
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rate);
    }
    private Vector3 MovePoint(Vector3 position)
    {
        float moveX = Random.Range(position.x - RANGE, position.x + RANGE);
        float moveZ = Random.Range(position.z - RANGE, position.z + RANGE);
        return new Vector3(moveX, this.transform.position.y, moveZ);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagName.Weapon))
        {
            Damage(hitDamage);
        }
    }
}
