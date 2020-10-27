using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectPoints : MonoBehaviour
{
    [System.Serializable]
    public struct EffectPoint
    {
        public string name;
        public Refarences.EBodyParts pointType;
        public GameObject pointObject;
    }
    
    [System.Serializable]
    public struct EffectJoints
    {
        public string name;
        public Refarences.BodyJoints jointType;
        public GameObject jointObject;
    }

    public EffectPoint[] points;
    [SerializeField] private EffectJoints[] joints;

    private Dictionary<Refarences.EBodyParts, GameObject> _effectPointsDic;
    private Dictionary<Refarences.BodyJoints, GameObject> _effectJointsDic;

    public void Awake() {
        _effectPointsDic = new Dictionary<Refarences.EBodyParts, GameObject>();
        _effectJointsDic = new Dictionary<Refarences.BodyJoints, GameObject>();
        foreach (var point in points) {
            _effectPointsDic.Add(point.pointType, point.pointObject);
        }
        foreach (var joint in joints) {
            _effectJointsDic.Add(joint.jointType, joint.jointObject);
        }
    }

    public Transform GetPointTransform(Refarences.EBodyParts pointName) {
        if (_effectPointsDic == null || _effectPointsDic.Count == 0) {
            return (from point in points where point.pointType == pointName select point.pointObject.transform).FirstOrDefault();
        }
        return _effectPointsDic[pointName].transform;
    }
    
    public GameObject GetPointObject(Refarences.EBodyParts pointName) {
        if (_effectPointsDic == null || _effectPointsDic.Count == 0) {
            return (from point in points where point.pointType == pointName select point.pointObject).FirstOrDefault();
        }
        return _effectPointsDic[pointName];
    }
    
    public Transform GetJointTransform(Refarences.BodyJoints jointType) {
        if (_effectJointsDic == null || _effectJointsDic.Count == 0) {
            return (from joint in joints where joint.jointType == jointType select joint.jointObject.transform).FirstOrDefault();
        }
        return null;
    }
    
    public GameObject GetJointObject(Refarences.BodyJoints jointType) {

        foreach (var joint in joints) {
            if (joint.jointType == jointType) {
                return joint.jointObject;
            }
        }
        
        if (_effectJointsDic == null || _effectJointsDic.Count == 0) {
            return (from joint in joints where joint.jointType == jointType select joint.jointObject).FirstOrDefault();
        }
        return null;
    }
}
