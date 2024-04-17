using System.Collections;
using System.Collections.Generic;
using _00_GameData.Scripts.Systems;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    PoolManager _poolManager;
    Transform target;
    private void Start()
    {
        _poolManager = GameObject.FindGameObjectWithTag(TagName.GameController).GetComponent<PoolManager>();
        target = GameObject.FindGameObjectWithTag(TagName.Player).GetComponent<Transform>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var forwerd = target.forward * 10;
            var pos = new Vector3(target.position.x + forwerd.x, target.position.y + forwerd.y, target.position.z + forwerd.z);
            _poolManager.InstantiateSuraimu(1, pos, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var forward = target.forward * 10;
            var pos = new Vector3(target.position.y + forward.x,
            target.position.y + forward.y,
            target.position.z + forward.z);
            _poolManager.InstantiateGingerman(1, pos, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var forward = target.forward * 10;
            var pos = new Vector3(target.position.y + forward.x,
            target.position.y + forward.y,
            target.position.z + forward.z);
            _poolManager.InstantiateHumanoid(1, pos, 2);
        }
    }
}
