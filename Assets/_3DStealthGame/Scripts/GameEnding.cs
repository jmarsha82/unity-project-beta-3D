using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public GameObject player;
    public UIDocument uiDocument;
    public AudioSource exitAudio;
    public AudioSource caughtAudio;
    public AudioClip exitClip;
    public AudioClip caughtClip;

    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    float m_Timer;
    bool m_HasAudioPlayed;

    private VisualElement m_EndScreen;
    private VisualElement m_CaughtScreen;

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogWarning($"{name} is missing a UIDocument reference.", this);
            return;
        }

        m_EndScreen = uiDocument.rootVisualElement.Q<VisualElement>("EndScreen");
        m_CaughtScreen = uiDocument.rootVisualElement.Q<VisualElement>("CaughtScreen");
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    public void CaughtPlayer ()
    {
        m_IsPlayerCaught = true;
    }

    void Update ()
    {
        if (m_IsPlayerAtExit)
        {
            EndLevel (m_EndScreen, false, exitAudio, exitClip);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel (m_CaughtScreen, true, caughtAudio, caughtClip);
        }
    }

    void EndLevel (VisualElement element, bool doRestart, AudioSource audioSource, AudioClip audioClip)
    {
        if (!m_HasAudioPlayed)
        {
            PlayEndingAudio(audioSource, audioClip);
            m_HasAudioPlayed = true;
        }

        m_Timer += Time.deltaTime;

        if (element != null)
        {
            element.style.opacity = m_Timer / fadeDuration;
        }

        if (m_Timer > fadeDuration + displayImageDuration)
        {
            if (doRestart)
            {
                SceneManager.LoadScene (0);
            }
            else
            {
                Application.Quit();
                Time.timeScale = 0;
            }
        }
    }

    void PlayEndingAudio(AudioSource audioSource, AudioClip audioClip)
    {
        if (audioSource != null)
        {
            audioSource.Play();
            return;
        }

        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
            return;
        }

        Debug.LogWarning($"{name} is missing an end-level audio source or audio clip.", this);
    }
}
