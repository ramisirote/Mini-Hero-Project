using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAngler : MonoBehaviour
{
    [SerializeField] private float smoothness;
    [SerializeField] private List<BodyPart> bodyParts;
    private Camera mainCam;

    private Vector3 toRotation;

    private Dictionary<Refarences.EBodyParts, Quaternion> defaultAngles = new Dictionary<Refarences.EBodyParts, Quaternion>();
    private Dictionary<Transform, Quaternion> bodyPartTargetAngle = new Dictionary<Transform, Quaternion>();

    private void Start() {
        if (mainCam == null) {
            mainCam = Camera.main;
        }

        foreach (var part in bodyParts) {
            defaultAngles[part.type] = part.transform.localRotation;
        }
    }
    
    public void ResetAngle(Refarences.EBodyParts partType) {
        RotatePart(partType, defaultAngles[partType]);
    }

    public void RotatePart(Refarences.EBodyParts partType, float angle) {
        var bodyPart = bodyParts.Find(part => part.type == partType);
        RotatePart(bodyPart.transform, angle);
    }
    
    public void RotatePart(Refarences.EBodyParts partType, Quaternion angle) {
        var bodyPart = bodyParts.Find(part => part.type == partType);
        RotatePart(bodyPart.transform, angle);
    }

    public void RotatePart(Transform bodyPart, float angle) {
        bodyPartTargetAngle[bodyPart] = Quaternion.Euler(new Vector3(0, 0,angle));
    }
    
    public void RotatePart(Transform bodyPart, Vector3 angle) {
        bodyPartTargetAngle[bodyPart] = Quaternion.Euler(angle);
    }

    public void RotatePart(Transform bodyPart, Quaternion angle) {
        bodyPartTargetAngle[bodyPart] = angle;
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

    [System.Serializable]
    public struct BodyPart
    {
        public Refarences.EBodyParts type;
        public Transform transform;
    }
}
