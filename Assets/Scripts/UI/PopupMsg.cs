using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMsg : MonoBehaviour
{
    public Text msgText;
    public float upSpeed;

    public PopupMsg Setup(string msg, float duration, Color color)
    {
        msgText.text = msg;
        msgText.color = color;

        transform.LookAt(Camera.main.transform, Vector3.up);
        StartCoroutine(MsgRoutine(duration));
        return this;
    }

    IEnumerator MsgRoutine(float duration)
    {
        float t = 0;
        Color initialColor = msgText.color;
        Color endColor = msgText.color;
        endColor.a = 0;
        while (t < 1)
        {
            msgText.color = Color.Lerp(initialColor, endColor, t);
            transform.position += Vector3.up * Time.deltaTime * upSpeed;
            t += Time.deltaTime / duration;
            yield return null;
        }
        Destroy(gameObject);
    }
}
