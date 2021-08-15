using UnityEngine;
using System.Collections;
using System;

public class CameraLook : MonoBehaviour
{

    public GameObject target;
    private Vector3 point;
    public float limitSlow = 15;
    public float limitAngle = 30;
    public float speed = 0.1f;

    bool isSwiping = false;
    float lastX;
    void Start()
    {
        point = target.transform.position;
        transform.LookAt(point);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            lastX = Input.mousePosition.x;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }
        if (isSwiping)
        {
            float curX = Input.mousePosition.x;
            Swipe(curX - lastX);
            lastX = curX;
        }
    }
    public void Swipe(float moveX)
    {
        float angle = transform.eulerAngles.y;
        float scale = 1;
        if (angle > 180) angle -= 360;
        if (moveX > 0 && angle > limitSlow) scale = (limitAngle - angle)/(limitAngle - limitSlow);
        if (moveX < 0 && angle < -limitSlow) scale = (limitAngle + angle) / (limitAngle - limitSlow);
        transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), speed * moveX * scale);
    }
}