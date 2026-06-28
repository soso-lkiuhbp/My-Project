using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMono : MonoBehaviour
{
    RectTransform _startBG;
    void Start()
    {
        _startBG = transform.Find("StartBtn/BG") as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        _startBG.RotateAround(_startBG.position,Vector3.forward,Time.deltaTime * 20);
    }
}
