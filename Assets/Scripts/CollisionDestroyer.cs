using UnityEngine;
using System.Collections;

public class CollisionDestroyer : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		GameObject target = col.gameObject;
		
		if (col.attachedRigidbody != null) target = col.attachedRigidbody.gameObject;
		
		Destroy(target);
	}
}
