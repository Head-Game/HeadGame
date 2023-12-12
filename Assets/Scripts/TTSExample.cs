using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodEnough.TextToSpeech;

public class TTSExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TTS.Speak("Hello World!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
