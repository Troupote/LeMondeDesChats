using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
	[SerializeField]
	private int nbEntity = 10;

	[SerializeField]
	private GameObject entityPrefab;

	void Start()
	{
		for (int i = 0; i < nbEntity; i++)
		{
			Instantiate(entityPrefab, new Vector3(0, 0 , 0), Quaternion.identity); 
		}
		
	}

	// Update is called once per frame
	void Update()
	{
	}
}
