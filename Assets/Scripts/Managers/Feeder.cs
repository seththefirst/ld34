using UnityEngine;
using System.Collections;

public class Feeder : MonoBehaviour
{

    void OnTriggerEnter( Collider other )
    {
        Debug.Log( other.name );
    }
}
