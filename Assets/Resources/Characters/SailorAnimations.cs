using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class SailorAnimations : MonoBehaviour
{
    public GameObject model;
    [SpineAnimation]
    public string idleAnimation;
    [SpineAnimation]
    public string attackAnimations;
    private SkeletonAnimation skel;
    void Awake()
    {
        if (model == null) model = transform.Find("model").gameObject;
        skel = model.GetComponent<SkeletonAnimation>();
    }
    private void Start()
    {
        skel.AnimationName = attackAnimations;
        skel.loop = false;
        skel.AnimationState.Event += HandleEvent;

        skel.AnimationState.Start += delegate (TrackEntry trackEntry) {
            // You can also use an anonymous delegate.
            Debug.Log(string.Format("track {0} started a new animation.", trackEntry.TrackIndex));
        };

        skel.AnimationState.End += delegate {
            // ... or choose to ignore its parameters.
            Debug.Log("An animation ended!");
            skel.AnimationName = idleAnimation;
            skel.loop = true;
        };
    }
    void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == attackAnimations)
        {
            Debug.Log("Play a footstep sound!");
        }
    }
    void Update()
    {
        
    }
}
