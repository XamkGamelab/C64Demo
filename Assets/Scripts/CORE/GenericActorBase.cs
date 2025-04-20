using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GenericActorBase : MonoBehaviour
{
    public ReactiveProperty<Vector3?> DeathPosition = new ReactiveProperty<Vector3?>(null);
    public int BulletHitCount = 0;    
    public SpriteRenderer SpriteRend => GetComponent<SpriteRenderer>();

    protected bool ignoreBullets { get; set; } = false;
    protected Action<GenericActorBase> hitAction = null;

    private new Collider2D collider2D = null;
    private Rigidbody2D rb;
    private Vector2? moveSpeed = Vector2.zero;
    private bool dieOnBulletCollision;
    
    public virtual GenericActorBase Init(Vector2? speed, Type colliderType, bool isTrigger = true, bool dieOnBullet = true)
    {
        if (colliderType == typeof(BoxCollider2D) || colliderType == typeof(CircleCollider2D))
        {
            if (colliderType == typeof(BoxCollider2D))
                collider2D = gameObject.AddComponent<BoxCollider2D>();
            else if (colliderType == typeof(CircleCollider2D))
                collider2D = gameObject.AddComponent<CircleCollider2D>();

            collider2D.isTrigger = isTrigger;
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        dieOnBulletCollision = dieOnBullet;
        moveSpeed = speed;
        return this;
    }

    public GenericActorBase IgnoreBullets(bool ignoreBulletHits)
    {
        ignoreBullets = ignoreBulletHits;
        return this;
    }

    public GenericActorBase AddHitAction(Action<GenericActorBase> action)
    {
        hitAction = action;
        return this;
    }

    public virtual void Die(bool destroyGO)
    {
        DeathPosition.Value = transform.position;
        if (destroyGO)
            Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ignoreBullets)
        {
            GenericBullet bullet = collision.GetComponent<GenericBullet>();
            if (bullet != null)
            {
                //Increment hit count
                BulletHitCount++;

                //Invoke Bullet hit triggered action with this enemy as parameter. If any.
                hitAction?.Invoke(this);

                //And also die if dieOnBulletCollision
                if (dieOnBulletCollision)
                    Die(true);

                bullet.Die();
            }
        }
    }

    protected virtual void Update()
    {
        if (moveSpeed.HasValue)
            transform.Translate(moveSpeed.Value * Time.deltaTime);
    }
}
