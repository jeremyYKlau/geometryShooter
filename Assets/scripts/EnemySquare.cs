using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySquare : Enemy {

    protected override void Awake()
    {
        base.Awake();
        //enemyType = Type.Square;
        enemyType = 1;
    }

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(UpdatePath());
    }

    protected override void Update()
    {
        base.Update();
    }

}
