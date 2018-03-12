using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class MasterController : MonoBehaviour {

    public PlayableDirector playableDirector;
    public MyMidi MidiPlayer;

    // Use this for initialization
    IEnumerator Start () {

        MidiPlayer.Init();

        // Wait for one second to avoid stuttering.
        yield return new WaitForSeconds(1.0f);

        Play();
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    private void Play()
    {
        MidiPlayer.Begin(0); 
        playableDirector.Play();
    }
}
