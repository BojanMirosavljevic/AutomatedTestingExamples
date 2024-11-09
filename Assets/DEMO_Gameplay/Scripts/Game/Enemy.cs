using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private SpriteRenderer crosshair;
    [SerializeField] private float speed;
    [SerializeField] private float health;

    private Transform target;

    void Start()
    {
        target = GameManager.Instance.Player.transform;
        crosshair.enabled = false;

        transform.SetParent(GameManager.Instance.Environment);
    }

    void FixedUpdate()
    {
        Vector2 dir = (target.transform.position - rigidBody2D.transform.position).normalized;
        rigidBody2D.MovePosition(rigidBody2D.position + (dir * speed * Time.fixedDeltaTime));
    }

    public void DoDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnMouseEnter()
    {
        crosshair.enabled = true;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.Player.TryShootEnemy(this);
        }
    }

    void OnMouseExit()
    {
        crosshair.enabled = false;
    }

    void OnDestroy()
    {
        GameManager.Instance.NotifyEnemyDestroyed();
    }
}
