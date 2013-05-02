using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TunePlayerVisualizer : MonoBehaviour
{
	public TunePlayer player;
	
	public Transform firstStartPos;
	public Transform firstEndPos;
	public Transform lastStartPos;
	public Transform lastEndPos;
	public Color lineColor1 = Color.white;
	public Color lineColor2 = Color.white;
	public GameObject noteObjectPrefab;
	public LineRenderer lineRendererPrefab;
	
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
		if (lineRendererObjects != null)
		{
			for (int i = 0; i < lineRendererObjects.Length; i++)
			{
				Destroy(lineRendererObjects[i]);
			}
		}
		
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
			Debug.Log(noteStartPositions[player.audioSources[i]]);
		}
		
		if (noteObjects != null)
		{
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
		if (noteStartPositions != null && noteStartPositions.ContainsKey(source))
		{
			GameObject go = Instantiate(
					noteObjectPrefab, 
					noteStartPositions[source], 
					Quaternion.identity) as GameObject;
			go.transform.parent = transform;
			noteObjects.Add(go.transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (noteObjects != null)
		{
			for (int i = 0; i < noteObjects.Count; i++)
			{
				if (noteObjects[i] == null)
				{
					noteObjects.RemoveAt(i);
					i--;
				}
				else
				{
					float interval = 1 / (noteObjects.Count + 1);
					noteObjects[i].position += objectVelocity * Time.deltaTime;
				}
			}
		}
		
	}
}
