using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    static public FXManager Instance { get; private set; }

    public PopupMsg popupPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    static public void ShowPopupAt(Vector3 pos, string msg, float duration, Color color)
    {
        var popup = Instantiate(Instance.popupPrefab, pos + Vector3.up * 3, Quaternion.identity, null);
        popup.Setup(msg, duration, color);
    }

}
