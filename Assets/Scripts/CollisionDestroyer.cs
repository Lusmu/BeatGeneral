using UnityEngine;
using System.Collections;

/// <summary>
/// Destroys Gameobjects on OnCollisionEnter and OnTriggerEnter.
/// </summary>
public class CollisionDestroyer : MonoBehaviour {
	
	void OnCollisionEnter(Collision col)
	{
		GameObject target = col.collider.gameObject;
		
		// We want to destroy the gameobject holding the rigidbody
		if (col.collider.attachedRigidbody != null) target = col.collider.attachedRigidbody.gameObject;
		
		Destroy(target);
	}
	
	void OnTriggerEnter(Collider col)
	{
		GameObject target = col.gameObject;
		
		// We want to destroy the gameobject holding the rigidbody
		if (col.attachedRigidbody != null) target = col.attachedRigidbody.gameObject;
		
		Destroy(target);
	}
}
