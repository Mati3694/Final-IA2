using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    static public World Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField]
    private WorldStateConfig config;

    static public WorldStateConfig Config { get { return Instance.config; } }

    public Transform bed;
    public Transform shop;
    public Transform castle;
    public Transform train;
    public BossModel boss;
}
