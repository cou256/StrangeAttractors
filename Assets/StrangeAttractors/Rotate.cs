using UnityEngine;

public class Rotate : MonoBehaviour
{
	void Update () {
        var ea = transform.eulerAngles;
        ea.y += 0.1f;
        transform.rotation = Quaternion.Euler(ea);
    }
}
