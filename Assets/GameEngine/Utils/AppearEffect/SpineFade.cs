using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineFade : MonoBehaviour
{
    Spine.Skeleton skeleton;
    float alpha = 1f;
    float remain = 2f;
    private void Awake()
    {
        var s1 = GetComponent<SkeletonAnimation>();
        var s2 = GetComponent<SkeletonMecanim>();
        var s3 = GetComponent<SkeletonGraphic>();
        if (s1) skeleton = s1.Skeleton;
        else if (s2) skeleton = s2.Skeleton;
        else if (s3) skeleton = s3.Skeleton;
        skeleton.A = 0;
    }
    public void SetAlpha(float a)
    {
        alpha = a;
        remain = 0;
        skeleton.A = a;
    }
    public void FadeTo(float value, float duration = .5f) {
        alpha = value;
        remain = duration;
    }
    private void LateUpdate()
    {
        if (skeleton == null) return;
        if (remain <= 0.001f)
        {
            skeleton.A = alpha;
            remain = 0f;
        }
        else
        {
            Debug.Log("skeleton.A " + skeleton.A);
            Debug.Log("remain " + remain);
            Debug.Log("alpha - skeleton.A " + (alpha - skeleton.A));
            Debug.Log("remain * Time.deltaTime " +  Time.deltaTime / remain);
            Debug.Log("step " + (alpha - skeleton.A) / remain * Time.deltaTime);
            //skeleton.A += (alpha - skeleton.A) / remain * Time.deltaTime;
            //remain -= Time.deltaTime;
        }
    }
}
