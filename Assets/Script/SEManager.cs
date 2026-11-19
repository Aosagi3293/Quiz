using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }

    public AudioSource audioSource;

    public AudioClip click;
    public AudioClip timer;
    public AudioClip answerShow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(click);
    }

    public void TimerSound()
    {
        audioSource.PlayOneShot(timer);
    }

    public void AnswerShowSound()
    {
        audioSource.PlayOneShot(answerShow);
    }
}
