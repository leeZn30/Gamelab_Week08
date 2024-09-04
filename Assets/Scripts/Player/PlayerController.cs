using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerContext playerContext;
    public int currentHP = 2;

    // Start is called before the first frame update
    void Start()
    {
        playerContext = new PlayerContext();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
