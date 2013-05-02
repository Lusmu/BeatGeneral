using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Draws lines and creates and translates objects
/// representing notes played by a TunePlayer.
/// </summary>
public class TunePlayerVisualizer : MonoBehaviour
{
	public TunePlayer player;
	
	/// <summary>
	/// The line start position.
	/// Also affects note object instatiating.
	/// </summary>
	public Transform firstStartPos;
	/// <summary>
	/// The line start position.
	/// Also affects note object instatiating.
	/// </summary>
	public Transform lastStartPos;
	
	/// <summary>
	/// The line end position.
	/// </summary>
	public Transform firstEndPos;
	/// <summary>
	/// The line end position.
	/// </summary>
	public Transform lastEndPos;
	
	public Color lineColor1 = Color.white;
	public Color lineColor2 = Color.white;
	
	public GameObject noteObjectPrefab;
	/// <summary>
	/// The line renderer prefab.
	/// Must not be null!
	/// </summary>
	public LineRenderer lineRendererPrefab;
	
	/// <summary>
	/// The velocity vector for note objects.
	/// </summary>
	public Vector3 objectVelocity = new Vector3(-2, 0, 0);
	
	GameObject[] lineRendererObjects;
	List<Transform> noteObjects;
	Dictionary<AudioSource, Vector3> noteStartPositions;
	
	void OnEnable()
	{
		player.OnInitDone += Init;
		player.OnPlayNote += OnPlayNote;
	}
	
	void OnDisable()
	{
		player.OnInitDone -= Init;
		player.OnPlayNote -= OnPlayNote;
	}
	
	void Init()
	{
		// If initialing more than once, destroy old line objects.
		if (lineRendererObjects != null)
		{
			for (int i = 0; i < lineRendererObjects.Length; i++)
			{
				Destroy(lineRendererObjects[i]);
			}
		}
		
		// Create a dictionary for spawn positions for audio sources.
		if (noteStartPositions != null) noteStartPositions.Clear();
		else noteStartPositions = new Dictionary<AudioSource, Vector3>();
		
		lineRendererObjects = new GameObject[player.audioSources.Length];
		Vector3 interval = (lastStartPos.position - firstStartPos.position) / (player.audioSources.Length + 1);
		
		for (int i = 0; i < lineRendererObjects.Length; i++)
		{
			lineRendererObjects[i] = Instantiate(lineRendererPrefab.gameObject) as GameObject;
			LineRenderer line = lineRendererObjects[i].GetComponent<LineRenderer>();
			lineRendererObjects[i].transform.parent = transform;
			line.SetPosition(0, firstStartPos.position + (i + 1) * interval + Vector3.forward * 0.1f);
			line.SetPosition(1, firstEndPos.position + (i + 1) * interval + Vector3.forward * 0.1f);
			line.SetColors(lineColor1, lineColor2);
			noteStartPositions.Add(player.audioSources[i], firstStartPos.position + (i + 1) * interval);
		}
		
		if (noteObjects != null)
		{
			// If initialing more than once, destroy old note objects.
			for (int i = 0; i < noteObjects.Count; i++)
			{
				Destroy(noteObjects[i].gameObject);
			}
			noteObjects.Clear();
		}
		else
		{
			noteObjects = new List<Transform>();
		}
		
	}
	
	void OnPlayNote(AudioSource source)
	{
		if (noteObjectPrefab != null && noteStartPositions != null && noteStartPositions.ContainsKey(source))
		{
			// Create a new note object.
			GameObject go = Instantiate(
					noteObjectPrefab, 
					noteStartPositions[source], 
					Quaternion.identity) as GameObject;
			go.transform.parent = transform;
			noteObjects.Add(go.transform);
		}
	}
	
	void Update ()
	{
		// Update note object positions
		if (noteObjects != null)
		{
			for (int i = 0; i < noteObjects.Count; i++)
			{
				if (noteObjects[i] == null)
				{
					// If note object has been destroyed, remove from list.
					noteObjects.RemoveAt(i);
					i--;
				}
				else
				{
					noteObjects[i].position += objectVelocity * Time.deltaTime;
				}
			}
		}
	}
}
