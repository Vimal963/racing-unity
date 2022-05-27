using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    public UnityEngine.UI.Image MusicImage;

    public Sprite On;

    public Sprite Off;

    public AudioSource musicSource;

    public AudioClip gamePlaySound;

    public AudioClip waitingForPlay;

    public AudioClip WinSound;

    public AudioClip LoseSound;

    string iMusic = "CarRacingIsMusic";

    //public bool IsMusic = true;


    public bool IsMusic
    {
        get
        {
            return (PlayerPrefs.GetInt(iMusic, 1) == 1) ? true : false;
        }
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt(iMusic, 1);
            }
            else
            {
                PlayerPrefs.SetInt(iMusic, 0);
            }
        }
    }

    private void Awake()
    {
        I = this;
    }


    private void Start()
    {
        if (IsMusic)
        {
            MusicImage.sprite = On;
        }
        else
        {
            MusicImage.sprite = Off;
        }
        MusicPlay();
    }


    void MusicPlay()
    {
        if (IsMusic)
        {
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void loseGamePlay()
    {

        if (IsMusic)
        {
            musicSource.clip = LoseSound;
            musicSource.loop = false;
            musicSource.Play();
        }
    }

    public void Win()
    {
        if (IsMusic)
        {
            musicSource.clip = WinSound;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void GamePlaySoundPlay()
    {
        if (IsMusic)
        {
            musicSource.clip = gamePlaySound;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void WaitingForReadyPlay()
    {
        if (IsMusic)
        {
            musicSource.clip = waitingForPlay;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void MusicChange()
    {

        if (IsMusic)
        {
            IsMusic = false;
            MusicImage.sprite = Off;
        }
        else
        {
            IsMusic = true;
            MusicImage.sprite = On;
        }
        MusicPlay();
    }


}