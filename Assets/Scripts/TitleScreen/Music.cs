using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.SaveGame.Scripts.SaveSystem;

namespace TitleScreen {
	public partial class TitleScreenHandler {
		void SetMusicVolumes(bool classicMusic, bool remasterMusic) {
			Dictionary<MusicTrack, float> volumes = new Dictionary<MusicTrack, float>() {
				{MusicTrack.Classic, classicMusic ? 1f : 0f},
				{MusicTrack.Remastered, remasterMusic ? 1f : 0f},
				{MusicTrack.Gba, (!classicMusic && !remasterMusic) ? 1f : 0f}
			};

			foreach ((MusicTrack key, float value) in volumes) {
				switch (key) {
					case MusicTrack.Classic:
						classic.volume = value;
						break;
					case MusicTrack.Remastered:
						remaster.volume = value;
						break;
					case MusicTrack.Gba:
						gba.volume = value;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		void PlayMusicTrack(MusicTrack track, float volume) {
			AudioSource[] tracks = {classic, remaster, gba};
			string[] keys = {"classic_music", "remastered_music", "gba_music"};

			for (int i = 0; i < tracks.Length; i++) {
				tracks[i].volume = (i == (int)track) ? volume : 0f;
				SaveSystem.SetBool(keys[i], i == (int)track);
				if (i == (int)track) {
					tracks[i].Play();
				}
			}

			SaveSystem.SaveToDisk();
		}

		public enum MusicTrack {
			Classic,
			Remastered,
			Gba
		}
	}
}
