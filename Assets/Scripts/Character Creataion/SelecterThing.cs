using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelecterThing : MonoBehaviour
{
    public Transform selectThing;
    public float offset;

    public void MoveSelectToMe() {
        selectThing.position = transform.position - new Vector3(offset, offset, 0);
    }
}
