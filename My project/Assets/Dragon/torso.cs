using UnityEngine;
using UnityEngine.InputSystem;

public class torso : MonoBehaviour, IDamageable
{
	[Header("Dragon Settings")]
	public float speed;
	public float currentHealth;
	public float maxHealth;
	[SerializeField] private GameObject Princess;
	[SerializeField] private HealthBar healthbar;

	[Header("Animation States")]
	private const string TORSO_IDLE = "torso_idle";
	private const string TORSO_RUN = "torso_run";
	private Vector2 moveDir;
	[SerializeField] private InputActionReference moveActionToUse;
	private SpriteRenderer dragonTorsoSprite;
	private Animator animator;
	private bool isFacingRight;
	private bool isMoving;
	private string currentState;

	void Start()
	{
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);
		dragonTorsoSprite = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (animator == null)
		{
			Debug.LogError("Animator component not found on " + gameObject.name);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (animator == null) return;
		HandleMovement();
	}

	private void UpdateDirection(Vector2 direction)
	{
		if (direction.x < 0)
		{
			dragonTorsoSprite.flipX = true;
			isFacingRight = false;
		}
		else
		{
			dragonTorsoSprite.flipX = false;
			isFacingRight = true;
		}
	}

	private void HandleMovement()
	{
		moveDir = moveActionToUse.action.ReadValue<Vector2>();
		if (moveDir != Vector2.zero)
		{
			ChangeAnimationState(TORSO_RUN);
			transform.Translate(moveDir * speed * Time.deltaTime);
			isMoving = true;
			UpdateDirection(moveDir);
		}
		else
		{
			isMoving = false;
			ChangeAnimationState(TORSO_IDLE);
		}
	}

	private void Die()
	{
		Debug.Log("Dragon died.");
	}

	private void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;

		animator.Play(newState);
		currentState = newState;
	}

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		healthbar.SetHealth(currentHealth);
		if (currentHealth <= 0)
		{
			Die();
		}
	}
}
