using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private Image cooldownIndicator;
    [SerializeField] private float speed;

    private Vector2 inputVector = new Vector2(0.0f, 0.0f);
    [SerializeField] private LineRenderer lineRenderer;
    private bool cooldown = true;
    private float shootCooldown = 0.6f;

    void Start()
    {
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        rigidBody2D.MovePosition(rigidBody2D.position + (inputVector * speed * Time.fixedDeltaTime));
    }

    public void TryShootEnemy(Enemy enemyHit)
    {
        if (cooldown)
        {
            cooldown = false;
            StartCoroutine(DoShoot(enemyHit));
        }
    }

    IEnumerator DoShoot(Enemy enemyHit)
    {
        lineRenderer.SetPosition(0, transform.position);

        if (enemyHit)
        {
            lineRenderer.SetPosition(1, enemyHit.transform.position);
            enemyHit.DoDamage();
        }
        else
        {
            Debug.LogError("Nothing hit");
            yield break;
        }


        lineRenderer.enabled = true;
        yield return new WaitForSecondsRealtime(0.05f);
        lineRenderer.enabled = false;

        yield return AnimateCooldownIndicator();
        cooldown = true;
    }

    IEnumerator AnimateCooldownIndicator()
    {
        float timePassed = 0f;
        cooldownIndicator.fillAmount = 1f;
        while (timePassed < shootCooldown)
        {
            timePassed += Time.unscaledDeltaTime;

            cooldownIndicator.fillAmount = Mathf.Lerp(1f, 0f, timePassed / shootCooldown);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Enemy"))
        {
            GameManager.Instance.StartEndGameFlow();
        }
    }
}
