using UnityEngine;

public class Fireball : MonoBehaviour
{
	public float damage = 50f;
	public float speed;
	public float lifetime;
	private Vector2 direction;
	private SpriteRenderer fireballRenderer;
	public float knockbackForce = 10f;

	void Start()
	{
		fireballRenderer = GetComponent<SpriteRenderer>();
		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		transform.Translate(direction * speed * Time.deltaTime);
		if (direction.x < 0)
		{
			fireballRenderer.flipX = true;
		}
		else if (direction.x > 0)
		{
			fireballRenderer.flipX = false;
		}
	}

	public void SetDirection(Vector2 dir)
	{
		direction = dir.normalized;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Princess") || collision.gameObject.CompareTag("enemy"))
		{
			IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(damage);
				// Apply knockback force
				Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
				if (rb != null)
				{
					Vector2 knockbackDirection = collision.transform.position - transform.position;
					knockbackDirection.Normalize();
					rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
				}
			}
		}
		Destroy(gameObject);
	}
}
