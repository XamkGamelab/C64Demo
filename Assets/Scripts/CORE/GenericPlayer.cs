using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPlayer : GenericActorBase
{
    protected bool dieOnEnemyCollision = true;
    public GenericActorBase Init(Vector2? speed, Type colliderType, bool isTrigger = true, bool dieOnBullet = true, bool dieOnEnemy = true)
    {
        dieOnEnemyCollision = dieOnEnemy;
        return base.Init(speed, colliderType, isTrigger, dieOnBullet);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        GenericEnemy enemy = collision.GetComponent<GenericEnemy>();
        if (enemy != null)
        {
            //Invoke hit triggered action with this player as parameter. If any.
            hitAction?.Invoke(this);

            //And also die if dieOnEnemyCollision
            if (dieOnEnemyCollision)
                Die(true);
        }
    }
}
