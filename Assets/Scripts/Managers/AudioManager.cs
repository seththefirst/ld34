using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioClip explosion;
    public AudioClip selection;
    public AudioClip giveCarrot;
    //

    void Awake()
    {
    }

    void Start()
    {

    }

    public void PlaySelection()
    {
        AudioSource.PlayClipAtPoint( selection, Camera.main.transform.position, 10f );
    }

    public void PlayCarrot()
    {
        AudioSource.PlayClipAtPoint( giveCarrot, Camera.main.transform.position, 10f );
    }

    public void PlayExplosion()
    {
        AudioSource.PlayClipAtPoint( explosion, Camera.main.transform.position, 1f );
    }

}
