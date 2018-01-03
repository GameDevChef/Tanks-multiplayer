using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioFX
{
    public string audioName;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("References")]

    [SerializeField]
    AudioSource m_ambientSource;

    [SerializeField]
    AudioSource m_musicSource;

    [SerializeField]
    AudioSource m_menuSource;

    AudioSO m_audioSO;

    Dictionary<string, int> m_FXDictionary = new Dictionary<string, int>();

    void Awake()
    {
        MakePersistentSingleton();
        LoadAudioClips();
    }

    void LoadAudioClips()
    {
        m_audioSO = Resources.Load<AudioSO>("AudioSO");
        for (int i = 0; i < m_audioSO.AudioFXList.Count; i++)
        {
            if (m_FXDictionary.ContainsKey(m_audioSO.AudioFXList[i].audioName))
            {
                Debug.LogError("audio error");
            }
            else
            {

                m_FXDictionary.Add(m_audioSO.AudioFXList[i].audioName, i);
            }
        }
    }

    void MakePersistentSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAmbient(string _name)
    {
        AudioClip clip = GetFX(_name);
        m_ambientSource.clip = clip;
        m_ambientSource.Play();
    }

    public void PlayMenu(string _name)
    {
        AudioClip clip = GetFX(_name);
        m_menuSource.clip = clip;
        m_menuSource.Play();
    }

    public void PlayMusic(string _name)
    {
        AudioClip clip = GetFX(_name);
        m_musicSource.clip = clip;
        m_musicSource.Play();
    }

    public AudioClip GetFX(string name)
    {
        int index = StringToInt(m_FXDictionary, name);
        if (index == -1)
            return null;

        return m_audioSO.AudioFXList[index].clip;
    }

    int StringToInt(Dictionary<string, int> _dictionary, string _name)
    {
        int index = -1;
        _dictionary.TryGetValue(_name, out index);

        return index;

    }
}
