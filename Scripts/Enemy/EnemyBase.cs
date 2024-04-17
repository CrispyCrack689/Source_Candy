using System;
using UnityEngine;
using AnnulusGames.LucidTools.Audio;
using _00_GameData.Scripts.Player;
using _00_GameData.Scripts.Systems;
using Cysharp.Threading.Tasks;

public class EnemyBase : MonoBehaviour
{
    const int ANIMTIME = 1;
    [SerializeField, Tooltip("アニメーター")]
    protected Animator animator;
    [SerializeField]
    protected AnimatorStateInfo animatorStateInfo;
    [Tooltip("敵のHP")]
    public int HP;
    [Tooltip("自身が貰うダメージ")]
    public int hitDamage;
    [Tooltip("自身の攻撃力")]
    public int ATK;
    [Tooltip("攻撃間隔")]
    public float attackDelay;
    [Tooltip("アニメーション待機ミリ秒")]
    public int attackAnimTime;
    [Tooltip("攻撃範囲")]
    public float attackDistance;
    [Tooltip("自身の速度")]
    public float Speed;
    [Tooltip("制限速度")]
    public float maxSpeed;
    [Tooltip("敵の視界(度)")]
    public float enemyAngle;
    [Tooltip("敵の視界(長さ)")]
    public float maxDistance;
    [Tooltip("目標へ向くまでの時間")]
    public float lookTime;
    private float timer = 0;
    [Range(-1, 1), Tooltip("回転方向")]
    public float searchingTime = 0;
    [Tooltip("攻撃音")]
    public AudioClip attackClip = null;
    [Tooltip("ダメージ音")]
    public AudioClip damageClip = null;

    public enum DropItem
    {
        HighItem1,
        HighItem2,
        HighItem3,
    }

    public DropItem dropItem;

    public enum EnemyState
    {
        Stay,
        Move,
        Attack,
    }

    public EnemyState enemyState;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public Transform target;
    private Vector3 selfPos;
    private Vector3 targetPos;
    private float lastAttackTime = 0;

    protected PoolManager _poolManager;
    private void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        _poolManager = GameObject.FindGameObjectWithTag(TagName.GameController).GetComponent<PoolManager>();
    }
    private void Start()
    {
        enemyState = EnemyState.Stay;
    }
    /// <summary>
    /// プレイヤーの検知
    /// </summary>
    /// <returns>発見時はTrue</returns>
    public bool IsVisible()
    {
        //  ポジションの取得
        selfPos = this.transform.position;
        targetPos = target.position;

        //  自身の方向
        var selfDir = this.transform.forward;

        //  対象までの距離と向き
        var targetDir = targetPos - selfPos;
        var targetDistance = targetDir.magnitude;

        //  cos/2の計算
        var cosHalf = Mathf.Cos(enemyAngle / 2 * Mathf.Deg2Rad);

        //  自身と対象の内積を計算
        var innerProduct = Vector3.Dot(selfDir, targetDir.normalized);

        //  判定
        return innerProduct > cosHalf && targetDistance < maxDistance;
    }
    /// <summary>
    /// 検知後のstate管理
    /// </summary>
    public void StateControl()
    {
        if (IsVisible() == true && enemyState == EnemyState.Stay)
        {
            enemyState = EnemyState.Move;
        }
        else if (IsVisible() == false && enemyState == EnemyState.Move)
        {
            animator.SetBool("Move", false);
            enemyState = EnemyState.Stay;
            rb.velocity *= 0f;
            rb.angularVelocity *= 0f;
        }
        if (AttackChance() == false && enemyState == EnemyState.Attack)
        {
            animator.SetBool("Wait", false);
            enemyState = EnemyState.Move;
        }
    }

    public virtual void Stay()
    {
        //  強制的に回転を修正
        if (transform.rotation.x != 0 || transform.rotation.z != 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
        }
        //  回転し索敵
        rb.angularVelocity = new Vector3(0, searchingTime, 0);
    }

    public virtual void Move()
    {
        animator.SetBool("Move", true);
        DirectionalTracking();
        rb.AddForce(this.transform.forward * Speed, ForceMode.Force);
        if (maxSpeed < rb.velocity.magnitude)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        if (AttackChance() == true)
        {
            animator.SetBool("Wait", true);
            enemyState = EnemyState.Attack;
            rb.velocity *= 0f;
            rb.angularVelocity *= 0f;
            lastAttackTime = Time.time;
        }

    }
    public virtual async void Attack()
    {
        DirectionalTracking();
        timer += Time.deltaTime;

        if (attackDelay <= timer && Time.time - lastAttackTime >= attackDelay)
        {
            animator.SetBool("Attack", true);
            lastAttackTime = Time.time;
        }

        if (animator.GetBool("Attack") == true)
        {
            await UniTask.Delay(attackAnimTime);
            playerController.Damage(ATK);
            LucidAudio.PlaySE(attackClip);
            timer = 0;
            animator.SetBool("Attack", false);




        }
    }

    public virtual async void Death()
    {
        // await UniTask.Delay(700);
        await UniTask.DelayFrame(1);
        print("アイテムDrop");
        EnemySpawner.enemyCount--;
        _poolManager.ReturnEnemy(this.gameObject);
    }

    /// <summary>
    /// 方向の追従
    /// </summary>
    private void DirectionalTracking()
    {
        var selgRotation = this.transform.rotation;
        var targetPos = new Vector3(target.position.x, target.position.y / 4, target.position.z);
        //  ベクトルの計算
        Vector3 direction = targetPos - this.transform.position;
        direction.y = 0;

        // 自身がターゲットの方向を向く
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
    }

    /// <summary>
    /// 攻撃可能かチェック
    /// </summary>
    /// <returns>発見ならTrue</returns>
    public bool AttackChance()
    {
        return Vector3.Distance(target.transform.position, this.transform.position) < attackDistance;
    }
    public virtual void Damage(int damage)
    {
        animator.SetBool("Hit", true);
        LucidAudio.PlaySE(damageClip);
        HP -= damage;
    }
    public void animReset()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.normalizedTime < ANIMTIME)
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Hit", false);
        }
    }
}