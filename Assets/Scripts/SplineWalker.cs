﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public BezierSpline spline;
    public float duration;
    private float progress;
    public bool lookFoeward;
    public SplineWalkerMode mode;
    private bool goingForward = true;       //记录物体是否前进

    private void Update()
    {
        if (goingForward) {

            progress += Time.deltaTime / duration;

            if (progress > 1f)
            {
                if(mode==SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if(mode==SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;
            if(progress < 0f )
            {
                progress = -progress;
                goingForward = true;
            }
        }
        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookFoeward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}
