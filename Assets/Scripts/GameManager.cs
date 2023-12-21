using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Pv.Unity;
using GoodEnough.TextToSpeech;
using System.IO;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Image fadeImage;

    public AudioSource audioSource;
    public AudioClip welcomeMusic;
    public string welcomeText;
    public string gameOverText;

    private Blade blade;
    private Spawner spawner;
    private PicovoiceManager _picovoiceManager;

    private int score;

    private void Awake()
    {
        blade = FindObjectOfType<Blade>();
        spawner = FindObjectOfType<Spawner>();

        // Picovoice
        string accessKey = "vIfZVH83r43ct+lGF5J8EWLMIhrwkr2JSk47MyOnHp9iGegW/iGksA==";
        string keywordPath = Path.Combine(Application.streamingAssetsPath, "Head-Game_en_ios_v3_0_0.ppn");
        string contextPath = Path.Combine(Application.streamingAssetsPath, "Fruit-Ninja-UI_en_ios_v3_0_0.rhn");
        _picovoiceManager = _picovoiceManager = PicovoiceManager.Create(
                                                accessKey,
                                                keywordPath,
                                                OnWakeWordDetected,
                                                contextPath,
                                                OnInferenceResult);


        // iOS TTS
        var speechParameters = new SpeechUtteranceParameters();
        speechParameters.PitchMultiplier = 1f;
        speechParameters.SpeechRate = 0.5f;
        speechParameters.Volume = 1f;
        speechParameters.PreUtteranceDelay = 0.1f;
        speechParameters.Voice = TTS.GetVoiceForLanguage("en-US");
    }

    private void OnWakeWordDetected()
    {
        // Handle wake word detection (e.g., "Hey Game")
        TTS.Speak("Yes?");
    }

    private void OnInferenceResult(Inference inference)
    {
        if(inference.IsUnderstood)
        {
            switch (inference.Intent)
            {
                case "startGame":
                    NewGame();
                    TTS.Speak("Starting Game Now");
                    break;
                case "restartGame":
                    RestartGame();
                    TTS.Speak("Restarting Game Now");
                    break;
                case "closeGame":
                    CloseGame();
                    TTS.Speak("Closing Game Now");
                    break;
                // Add other intents as needed
            }
        }
        else
        {
            // Handle unsupported commands
            TTS.Speak("Sorry, I can't understand.");
        }
    }

    private void Start()
    {
        try
        {
            _picovoiceManager.Start();
            StartCoroutine(WelcomeSequence());
        }
        catch(PicovoiceException ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    private IEnumerator WelcomeSequence()
    {
        audioSource.clip = welcomeMusic;
        audioSource.Play();

        Invoke("StopMusic", 5f);
        yield return new WaitForSeconds(5f);

        TTS.Speak(welcomeText);
    }

    private void StopMusic()
    {
        audioSource.Stop();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        // Reset Game
        spawner.ResetGame();

        // Start spawning fruits
        spawner.StartSpawning();

        score = 0;
        scoreText.text = score.ToString();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits) {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs) {
            Destroy(bomb.gameObject);
        }
    }

    private void RestartGame()
    {
        NewGame();
        // Additional code for restarting the game
    }

    private void CloseGame()
    {
        Application.Quit();
        // Additional code for closing the game
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        ClearScene();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }

    public void OnGameOver()
    {
        TTS.Speak(string.Format(gameOverText, score));
    }
}
