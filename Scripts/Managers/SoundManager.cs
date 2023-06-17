using CodeStage.AntiCheat.Storage;
using UnityEngine;
using static Define;

public class SoundManager
{
    private AudioSource[] audioSources = new AudioSource[(int)SoundType.Max];
    private GameObject soundRoot = null;
    public bool isMute;

    public void Init()
    {
		if (soundRoot == null)
		{
			soundRoot = GameObject.Find("@SoundRoot");
			if (soundRoot == null)
			{
				soundRoot = new GameObject { name = "@SoundRoot" };
				Object.DontDestroyOnLoad(soundRoot);

				string[] soundTypeNames = System.Enum.GetNames(typeof(SoundType));
				for (int count = 0; count < soundTypeNames.Length - 1; count++)
				{
					GameObject go = new GameObject { name = soundTypeNames[count] };
					audioSources[count] = go.AddComponent<AudioSource>();
					go.transform.parent = soundRoot.transform;
				}

				audioSources[(int)SoundType.BGM].loop = true;
			}
		}
		
		isMute = (ObscuredPrefs.Get("Mute", 0) == 1);
		for (int i = 0; i < audioSources.Length; i++)
		{
			AudioSource audioSource = audioSources[i];
			audioSource.mute = isMute;
		}
    }

    public void SetPitch(SoundType type, float pitch = 1.0f)
	{
		AudioSource audioSource = audioSources[(int)type];
        if (audioSource == null)
            return;

        audioSource.pitch = pitch;
	}

    public void ToggleMute()
    {
	    isMute = !isMute;
	    ObscuredPrefs.Set("Mute", isMute ? 1 : 0);

	    for (int i = 0; i < audioSources.Length; i++)
	    {
		    AudioSource audioSource = audioSources[i];
		    audioSource.mute = isMute;
	    }
    }

    public bool Play(SoundType type, string path, float volume = 1.0f, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        AudioSource audioSource = audioSources[(int)type];
        if (path.Contains("Sounds/") == false)
            path = string.Format("Sounds/{0}", path);

        if (type == SoundType.BGM)
        {
            AudioClip audioClip = Managers.Resource.Load<AudioClip>(path);
            if (audioClip == null)
                return false;

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
            return true;
        }
        else if (type == SoundType.SFX)
        {
            AudioClip audioClip = GetAudioClip(path);
            if (audioClip == null)
                return false;
            
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            return true;
        }

        return false;
    }

    public void Stop(SoundType type)
	{
        AudioSource audioSource = audioSources[(int)type];
        audioSource.Stop();
    }

	public float GetAudioClipLength(string path)
    {
        AudioClip audioClip = GetAudioClip(path);
        if (audioClip == null)
            return 0.0f;
        return audioClip.length;
    }

    private AudioClip GetAudioClip(string path)
    {
	    if (path.Contains("Sounds/") == false)
		    path = string.Format("Sounds/{0}", path);
	    return Managers.Resource.Load<AudioClip>(path);
    }
    
    public void Clear()
    {
	    foreach (AudioSource audioSource in audioSources)
		    audioSource.Stop();
    }
}