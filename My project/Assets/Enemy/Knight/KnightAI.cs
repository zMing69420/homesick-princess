using System.Collections;
using UnityEngine;

public class KnightAI : MonoBehaviour, IDamageable
{
	private GameObject Dragon;
	public float speed;
	public float health = 100f;
	public float attackRange;
	public float damage = 20f;
	public float attackCooldown = 1f; // Added cooldown time

	private SpriteRenderer mySpriteRenderer;
	private Animator animator;
	private bool isFacingRight;
	private string currentState;
	private Coroutine attackCoroutine;
	private const string KNIGHT_WALK_LEFT = "Knight_walk_left";
	private const string KNIGHT_WALK_RIGHT = "Knight_walk_right";
	private const string KNIGHT_HURT_LEFT = "Knight_hurt_left";
	private const string KNIGHT_HURT_RIGHT = "Knight_hurt_right";
	private const string KNIGHT_ATTACK_LEFT = "Knight_attack_left";
	private const string KNIGHT_ATTACK_RIGHT = "Knight_attack_right";

	private bool canAttack = true; // Added to control attack cooldown

	private void Start()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (animator == null)
		{
			Debug.LogError("Animator component not found on " + gameObject.name);
		}

		Dragon = GameObject.FindWithTag("Dragon");
		if (Dragon == null)
		{
			Debug.LogError("No GameObject with tag 'Dragon' found in the scene.");
		}
	}

	private void FixedUpdate()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		Vector2 direction = Dragon.transform.position - transform.position;
		float distance = direction.magnitude;
		if (distance <= attackRange)
		{
			if (canAttack)
			{
				HandleAttack();
			}
		}
		else
		{
			MoveTowardsDragon(direction);
		}
	}

	private void HandleAttack()
	{
		Vector2 direction = Dragon.transform.position - transform.position;
		if (direction.x < 0)
		{
			isFacingRight = false;
			ChangeAnimationState(KNIGHT_ATTACK_LEFT);
		}
		else if (direction.x > 0)
		{
			isFacingRight = true;
			ChangeAnimationState(KNIGHT_ATTACK_RIGHT);
		}

		IDamageable damageable = Dragon.GetComponent<IDamageable>();
		if (damageable != null)
		{
			damageable.TakeDamage(damage);
			Debug.Log("Damage dealt: " + damage);
		}
		else
		{
			Debug.Log("No damageable target found.");
		}

		// Start the attack cooldown
		StartCoroutine(AttackCooldown());
	}

	private IEnumerator AttackCooldown()
	{
		canAttack = false;
		yield return new WaitForSeconds(attackCooldown);
		canAttack = true;
	}

	private void MoveTowardsDragon(Vector2 direction)
	{
		transform.position = Vector2.MoveTowards(transform.position, Dragon.transform.position, speed * Time.deltaTime);

		if (direction.x < 0)
		{
			isFacingRight = false;
			ChangeAnimationState(KNIGHT_WALK_LEFT);
		}
		else if (direction.x > 0)
		{
			isFacingRight = true;
			ChangeAnimationState(KNIGHT_WALK_RIGHT);
		}
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Die();
		}
		else
		{
			PlayHurtAnimation();
		}
	}

	private void PlayHurtAnimation()
	{
		string hurtAnimation = isFacingRight ? KNIGHT_HURT_RIGHT : KNIGHT_HURT_LEFT;
		ChangeAnimationState(hurtAnimation);
		speed = 0;
		StartCoroutine(ChangeToRunningAnimationAfterDelay());
	}

	private IEnumerator ChangeToRunningAnimationAfterDelay()
	{
		yield return new WaitForSeconds(0.5f);
		speed = 1.5f;
		if (currentState == KNIGHT_HURT_LEFT || currentState == KNIGHT_HURT_RIGHT)
		{
			HandleMovement();
		}
	}

	private void ChangeAnimationState(string newState)
	{
		if (currentState != newState)
		{
			animator.Play(newState);
			currentState = newState;
		}
	}

	private void Die()
	{
		GetComponent<LootBag>().InstantiateLoot(transform.position);
		Destroy(gameObject);
	}
}
