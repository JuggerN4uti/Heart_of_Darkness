using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
	public Image effect;
	public float duration, fade;
	float fading;

	void Start()
	{
		fading = fade / duration;
	}

	void Update()
	{
		fade -= fading * Time.deltaTime;
		if (fade <= 0f)
			Destroy(gameObject);
		effect.color = new Color(1f, 1f, 1f, fade);
	}
}
