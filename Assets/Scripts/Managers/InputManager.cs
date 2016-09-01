using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private GameManager _gameManager;
    private AudioManager _audioManager;


    void Awake()
    {
        _gameManager = ( GameManager )FindObjectOfType( typeof( GameManager ) );
        _audioManager = ( AudioManager )FindObjectOfType( typeof( AudioManager ) );
    }

    void Start()
    {
	
    }


    void Update()
    {
        // Select next bunny
        if ( Input.GetKeyDown( KeyCode.A ) )
        {
            if ( _gameManager.gameOver )
            {
                _gameManager.BackToMainMenu();
            }
            else
            { 
                _gameManager.SelectNextBunny();
                _audioManager.PlaySelection();
            }
        }

        // Give Carrot
        if ( Input.GetKeyDown( KeyCode.D ) )
        {
            _gameManager.GiveCarrot( _gameManager.SelectedBunny );
            _audioManager.PlayCarrot();
        }

        if ( Input.GetKeyDown( KeyCode.Escape ) )
        {
            if ( !_gameManager.paused )
            {
                _gameManager.Pause();
            }
            else
            { 
                _gameManager.UnPause();
            }
        }
    }
}
