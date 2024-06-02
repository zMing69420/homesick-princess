using UnityEngine;

public class PrincessAI : MonoBehaviour, IDamageable
{
	[SerializeField] private float speed;
	public float currentHealth;
	public float maxHealth;
	[SerializeField] private HealthBar healthbar;
	[SerializeField] private GameObject Dragon;
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private BoxCollider2D playerCol;
	[SerializeField] private Animator animator;

	private bool isHolding = false;
	private GameObject heldItem = null;
	private GameObject target = null;
	private bool isFacingRight = true;
	private string currentState;

	private const string PRINCESS_IDLE_LEFT = "Princess_idle_left";
	private const string PRINCESS_IDLE_RIGHT = "Princess_idle_right";
	private const string PRINCESS_RUN_LEFT = "Princess_run_left";
	private const string PRINCESS_RUN_RIGHT = "Princess_run_right";
	private const string RIGHT_HOLDING = "right_holding";
	private const string LEFT_HOLDING = "left_holding";
	private const string LEFT_HOLDING_RUNNING = "left_holding_running";
	private const string RIGHT_HOLDING_RUNNING = "right_holding_running";

	void Start()
	{
		ValidateComponents();
		currentHealth = maxHealth;
		healthbar.SetMaxHealth(maxHealth);
	}

	void Update()
	{
		if (isHolding)
		{
			HandleHolding();
		}
		else
		{
			TrackLoot();
		}
	}

	private void ValidateComponents()
	{
		if (animator == null) Debug.LogError("Animator is not assigned.");
		if (rb == null) Debug.LogError("Rigidbody2D is not assigned.");
		if (playerCol == null) Debug.LogError("BoxCollider2D is not assigned.");
		if (Dragon == null) Debug.LogError("Dragon is not assigned.");
	}

	private void HandleHolding()
	{
		if (heldItem != null)
		{
			heldItem.GetComponent<PickableItem>().PickupItem(transform, isFacingRight);
			ChangeAnimationState(isFacingRight ? PRINCESS_IDLE_RIGHT : PRINCESS_IDLE_LEFT);
		}
	}

	private void TrackLoot()
	{
		GameObject nearestLoot = FindNearestLoot();
		if (nearestLoot != null)
		{
			MoveTowardsTarget(nearestLoot);
		}
		else
		{
			ChangeAnimationState(isFacingRight ? PRINCESS_IDLE_RIGHT : PRINCESS_IDLE_LEFT);
		}
	}

	private GameObject FindNearestLoot()
	{
		GameObject[] lootsInScene = GameObject.FindGameObjectsWithTag("pickableItem");
		float closestDistance = Mathf.Infinity;
		GameObject nearestLoot = null;
		torso dragonTorso = Dragon.GetComponent<torso>();

		// First, prioritize health potions if dragon health is not full
		if (dragonTorso != null && dragonTorso.currentHealth < dragonTorso.maxHealth)
		{
			foreach (GameObject pickableItem in lootsInScene)
			{
				if (pickableItem.name == "Health Potion")
				{
					float distanceToHealthPotion = Vector2.Distance(transform.position, pickableItem.transform.position);
					if (distanceToHealthPotion < closestDistance)
					{
						closestDistance = distanceToHealthPotion;
						nearestLoot = pickableItem;
					}
				}
			}
		}

		// If no health potions are found, look for the nearest other loot item
		if (nearestLoot == null)
		{
			foreach (GameObject pickableItem in lootsInScene)
			{
				float distance = Vector2.Distance(transform.position, pickableItem.transform.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					nearestLoot = pickableItem;
				}
			}
		}

		return nearestLoot;
	}

	private void MoveTowardsTarget(GameObject targetLoot)
	{
		target = targetLoot;
		Vector2 targetDirection = (target.transform.position - transform.position).normalized;
		transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
		UpdateDirection(targetDirection);
		ChangeAnimationState(isFacingRight ? PRINCESS_RUN_RIGHT : PRINCESS_RUN_LEFT);
	}

	private void UpdateDirection(Vector2 direction)
	{
		isFacingRight = direction.x >= 0;
	}

	private void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;
		animator.Play(newState);
		currentState = newState;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("pickableItem") && !isHolding)
		{
			PickUpItem(other.gameObject);
		}
	}

	private void PickUpItem(GameObject item)
	{
		isHolding = true;
		heldItem = item;
		heldItem.GetComponent<PickableItem>().PickupItem(transform, isFacingRight);
		Debug.Log("Picked up: " + item.name);
		UseItem();
	}

	private void UseItem()
	{
		if (heldItem == null || Dragon == null) return;

		IDamageable dragonTorso = Dragon.GetComponent<IDamageable>();
		if (dragonTorso == null) return;

		if (heldItem.name == "Health Potion")
		{
			UseHealthPotion(dragonTorso);
		}
		else
		{
			UseOtherItem();
		}
	}

	private void UseHealthPotion(IDamageable dragonTorso)
	{
		torso dragon = Dragon.GetComponent<torso>();
		if (dragon != null && dragon.currentHealth < dragon.maxHealth && currentHealth > dragon.currentHealth)
		{
			dragonTorso.TakeDamage(-20); // Heal the dragon
			Debug.Log("Used item: Health Potion, for dragon");
		}
		else if (currentHealth <= dragon.currentHealth)
		{
			TakeDamage(-20); // Heal the princess
			Debug.Log("Used item: Health Potion, for princess");
		}

		DestroyHeldItem();
	}

	private void UseOtherItem()
	{
		Debug.Log("Used other item");
		DestroyHeldItem();
	}

	private void DestroyHeldItem()
	{
		Destroy(heldItem);
		isHolding = false;
		heldItem = null;
	}

	public void TakeDamage(float damage)
	{
		Debug.Log($"Princess took {damage} damage."); // Debug log to verify damage
		currentHealth -= damage;
		healthbar.SetHealth(currentHealth);
		Debug.Log($"Princess current health: {currentHealth}"); // Debug log to verify current health
		if (currentHealth <= 0)
		{
			Debug.Log("Dead Princess");
		}
	}
}
