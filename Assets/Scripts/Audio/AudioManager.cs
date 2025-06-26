using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Cyberspeed.CardMatch.Enums;

namespace Cyberspeed.CardMatch.Audio
{
	[Serializable]
	public class Sound
	{
		public SoundType _type;
		public AudioClip _clip;
	}

	///<summary>
	/// Manages audio playback for the game.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }

		[Header("Sound Effects")]
		[SerializeField] private List<Sound> _sfxList;
		[SerializeField] private AudioSource _sfxAudioSource;

		/// <summary>
		/// Initializes the AudioManager singleton.
		/// </summary>
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			_sfxAudioSource = GetComponent<AudioSource>();
		}

		/// <summary>
		/// Plays a sound effect based on the specified type.
		/// </summary>
		public void PlaySfx(SoundType soundType)
		{
			var sound = _sfxList.FirstOrDefault(s => s._type == soundType);
			if (sound != null && sound._clip != null)
			{
				_sfxAudioSource.PlayOneShot(sound._clip);
			}
			else
			{
				Debug.LogWarning($"Sound of type {soundType} not found or clip is null.");
			}
		}
	}
}