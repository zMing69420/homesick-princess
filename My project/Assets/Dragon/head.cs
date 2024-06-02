using UnityEngine;

public class head : MonoBehaviour
{
	public GameObject fireballPrefab;
	private Vector3 firePoint;
	public float attackRange;

	[Header("Animation States")]
	const string HEAD_ATTACK = "head_attack";
	const string HEAD_IDLE = "head_idle";

	private SpriteRenderer dragonHeadSprite;
	private Animator animator;
	private GameObject target;
	private bool isFacingRight;
	private bool isMoving;
	private string currentState;

	void Start()
	{
		dragonHeadSprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (animator == null)
		{
			Debug.LogError("Animator component not found on " + gameObject.name);
		}
	}

	void Update()
	{
		HandleTargeting();
	}

	private void UpdateDirection(Vector2 direction)
	{
		if (direction.x < 0)
		{
			dragonHeadSprite.flipX = true;
			isFacingRight = false;
		}
		else
		{
			dragonHeadSprite.flipX = false;
			isFacingRight = true;
		}
	}

	private void HandleTargeting()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
		if (enemies.Length == 0)
		{
			ChangeAnimationState(HEAD_IDLE);
			return;
		}

		GameObject nearestEnemy = null;
		float closestDistance = Mathf.Infinity;

		foreach (GameObject enemy in enemies)
		{
			float distance = Vector2.Distance(transform.position, enemy.transform.position);
			if (distance < closestDistance)
			{
				closestDistance = distance;
				nearestEnemy = enemy;
			}
		}

		if (nearestEnemy != null && !isMoving && closestDistance <= attackRange)
		{
			target = nearestEnemy;
			Vector2 targetDirection = (target.transform.position - transform.position).normalized;
			UpdateDirection(targetDirection);
			ChangeAnimationState(HEAD_ATTACK);
		}
		else
		{
			target = null;
			ChangeAnimationState(HEAD_IDLE);
		}
	}

	private void LaunchFireballFromAnimationEvent()
	{
		LaunchFireball();
	}

	private void LaunchFireball()
	{
		if (fireballPrefab == null)
		{
			Debug.LogError("Fireball prefab is not assigned");
			return;
		}

		if (target == null)
		{
			Debug.LogError("Target is not assigned");
			return;
		}

		firePoint = isFacingRight ? new Vector3(0.7f, -0.1f, 0) : new Vector3(-0.7f, -0.1f, 0);
		GameObject fireball = Instantiate(fireballPrefab, transform.position + firePoint, Quaternion.identity);

		Vector2 fireballDirection = (target.transform.position - fireball.transform.position).normalized;
		fireball.GetComponent<Fireball>().SetDirection(fireballDirection);
	}

	private void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;

		animator.Play(newState);
		currentState = newState;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}
}
