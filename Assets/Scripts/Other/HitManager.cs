using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    public static HitManager instance;

    private Dictionary<GameObject, ITakeDamage> damageDic = new Dictionary<GameObject, ITakeDamage>();
    
    // Start is called before the first frame update
    void Awake() {
        if (!instance) instance = this;
    }

    public static ITakeDamage GetTakeDamage(GameObject ob) {
        return instance.damageDic.ContainsKey(ob) ? instance.damageDic[ob] : null;
    }

    public static void AddTakeDamage(GameObject ob) {
        instance.damageDic[ob] = ob.GetComponent<ITakeDamage>();
    }
    
    public static void AddTakeDamage(GameObject ob, ITakeDamage takeDamage) {
        instance.damageDic[ob] = takeDamage;
    }

    public static void RemoveTakeDamage(GameObject ob) {
        instance.damageDic.Remove(ob);
    }
}
