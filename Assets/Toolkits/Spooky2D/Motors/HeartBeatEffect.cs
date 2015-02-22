using UnityEngine;
using System.Collections;

public class HeartBeatEffect : MonoBehaviour {

    Color materialColor;
    public AnimationCurve beatAnimator;
    public float beatSpeed = 1;
    void Start()
    {
        materialColor = transform.renderer.material.color;
    }
    void LateUpdate()
    {
        Color newColor = new Color (materialColor.r,materialColor.b,materialColor.g,
            materialColor.a * beatAnimator.Evaluate(Time.timeSinceLevelLoad * beatSpeed));
        transform.renderer.material.color = newColor;


    }
}
