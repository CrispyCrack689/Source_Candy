using UnityEngine;

public class EnemySuraimu : EnemyBase
{
    public const int _HP = 30;
    private void Update()
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
        animReset();
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
}
