using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
