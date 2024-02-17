using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Image BlackFade;
    public float transparencyValue, fadeSpeed;
    public bool active;

    public void StartDarken()
    {
        active = true;
        Invoke("StartFade", 0.4f);
    }

    public void StartFade()
    {
        fadeSpeed *= -1f;
        Invoke("EndFade", 0.4f);
    }

    public void EndFade()
    {
        active = false;
        fadeSpeed *= -1f;
    }

    void Update()
    {
        if (active)
        {
            transparencyValue += fadeSpeed * Time.deltaTime;
            BlackFade.color = new Color(0f, 0f, 0f, transparencyValue);
        }
    }
}
