using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using SmfLite;

public class MyMidi : MonoBehaviour {

    public int bpm = 60;

    // Source MIDI file asset.
    public TextAsset sourceFile;

    MidiFileContainer song;
    MidiTrackSequencer seq;

    public MidiPlayDebug Kick;
    public MidiPlayDebug Snare;
    public MidiPlayDebug Hat;

    public void Init()
    {
        song = MidiFileLoader.Load(sourceFile.bytes);
        seq = new MidiTrackSequencer(song.tracks[0], song.division, bpm);
        Kick.Init();
        Snare.Init();
        Hat.Init();

    }

    // Reset and start sequecing.
    void ResetAndPlay(float startTime)
    {
        // Start the sequencer and dispatch events at the beginning of the track.
        seq = new MidiTrackSequencer(song.tracks[0], song.division, bpm);
        DispatchEvents(seq.Start(startTime));
    }

    public void Begin(float startTime)
    {
        DispatchEvents(seq.Start(startTime));
    }

    // Update function (MonoBehaviour).
    void Update()
    {
        if (seq != null && seq.Playing)
        {
            // Update the sequencer and dispatch incoming events.
            DispatchEvents(seq.Advance(Time.deltaTime));
            Timer += Time.deltaTime;

        }
    }

    /// <summary>
    /// A MIDI command plus its MIDI data parameters to be called a MIDI message . The minimum size of a MIDI message is 1 byte (one command byte and no parameter bytes). The maximum size of a MIDI message (note considering 0xF0 commands) is three bytes. A MIDI message always starts with a command byte.
    /// 
    /// Information for MIDI: https://ccrma.stanford.edu/~craig/articles/linuxmidi/misc/essenmidi.html
    /// The MIDI file format: https://www.csie.ntu.edu.tw/~r92092/ref/midi/
    /// 
    /// MIDI commands
    /// 0x80     Note Off
    /// 0x90     Note On
    /// 0xA0     Aftertouch
    /// 0xB0     Continuous controller
    /// 0xC0     Patch change
    /// 0xD0     Channel Pressure
    /// 0xE0     Pitch bend
    /// 0xF0     (non-musical commands)
    /// </summary>
    /// <param name="events"></param>

    float Timer;

    // Dispatch incoming MIDI events. 
    void DispatchEvents(List<MidiEvent> events)
    {
        if (events != null)
        {
            foreach (var e in events)
            {

                // Midi bytes from 10000000 to 11111111 are command bytes. SmfLite stores them in the status variable
                // Command bytes are split as well into two parts, The most significant half contains the actual MIDI command, and the second half contains the MIDI channel for which the command is fo 
                // If you AND a byte with 0xf0 (11110000), you get just the first half of the message, so here we are getting the actual MIDI command from our status byte
                // x90 for signifies a Note On message.
                //
                // If note On
                if ((e.status & 0xf0) == 0x90)
                {
                    //Debug.Log(Timer + " " + e.data1 + " " + e.data2);

                    if(e.data1 == 0x3C)
                    {
                        Kick.Play();
                    }
                    if(e.data1 == 0x3E)
                    {
                        Snare.Play();
                    }
                    if (e.data1 == 0x40)
                    {
                        Hat.Play();
                    }   
                }

            }
        }
    }

}
