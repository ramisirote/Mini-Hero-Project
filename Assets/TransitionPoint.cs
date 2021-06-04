using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [SerializeField] private string pointName;

    public bool IsPoint(string str) {
        return str == pointName;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
}
