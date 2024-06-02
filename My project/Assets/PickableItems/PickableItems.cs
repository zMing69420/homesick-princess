using UnityEngine;

public class PickableItem : MonoBehaviour
{
	private Transform playerTransform;
	private bool isPickedUp = false;
	public Vector3 offsetRight;
	public Vector3 offsetLeft;
	private Vector3 currentOffset;

	void Update()
	{
		if (isPickedUp && playerTransform != null)
		{
			// Use TransformPoint to maintain the local offset without scaling issues
			Vector3 targetPosition = playerTransform.TransformPoint(currentOffset);
			transform.position = targetPosition; // Follow the player
		}
	}

	public void PickupItem(Transform playerPos, bool isFacingRight)
	{
		playerTransform = playerPos;
		isPickedUp = true;
		currentOffset = isFacingRight ? offsetRight : offsetLeft;
	}
}
