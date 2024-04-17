using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    const float SPEED = 0.005f;
    [SerializeField] private Slider HPBar;
    [SerializeField] private Slider HPBackGround;
    private GameObject _camera;
    private EnemyBase enemyBase;
    private int baseHP;
    private void Start()
    {
        _camera = GameObject.FindWithTag(TagName.MainCamera);
        enemyBase = this.transform.root.gameObject.GetComponent<EnemyBase>();
        baseHP = enemyBase.HP;
        HPBar.value = 1;
        HPBackGround.value = 1;
    }
    private void Update()
    {
        _cameraLook();
        HPBarFix();
        if (GapCheck(HPBar.value, HPBackGround.value))
        {
            HPBackGround.value -= SPEED;
        }
    }

    private void HPBarFix()
    {
        float _value = (float)enemyBase.HP / (float)baseHP;
        HPBar.value = _value;
    }
    private void _cameraLook()
    {
        this.transform.LookAt(_camera.transform);
    }
    private bool GapCheck(float hpBar, float backGround)
    {
        var gap = backGround - hpBar;
        if (gap <= 0)
        {
            return false;
        }
        return true;
    }
}
