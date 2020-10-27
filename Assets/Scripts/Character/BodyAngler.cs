using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAngler : MonoBehaviour
{
    [SerializeField] private float smoothness;
    private Camera mainCam;

    private Vector3 toRotation;

    private Dictionary<Transform, Quaternion> bodyPartTargetAngle = new Dictionary<Transform, Quaternion>();

    private void Start() {
        if (mainCam == null) {
            mainCam = Camera.main;
        }
    }

    public void RotatePart(Transform bodyPart, float angle) {
        bodyPartTargetAngle[bodyPart] = Quaternion.Euler(new Vector3(0, 0,angle));
    }
    
    public void RotatePart(Transform bodyPart, Vector3 angle) {
        bodyPartTargetAngle[bodyPart] = Quaternion.Euler(angle);
    }
    
    private void FixedUpdate() {
        foreach (var bodyPartAngle in bodyPartTargetAngle) {
            if (!IsClose(bodyPartAngle.Value, bodyPartAngle.Key.localRotation)) {
                bodyPartAngle.Key.localRotation
                    = Quaternion.Lerp(bodyPartAngle.Key.localRotation, bodyPartAngle.Value,
                        Time.fixedDeltaTime * smoothness);
            }
        }
    }

    private bool IsClose(Quaternion a, Quaternion b) {
        return Math.Abs(a.eulerAngles.z%360 - b.eulerAngles.z%360) < 1;
    }
}
