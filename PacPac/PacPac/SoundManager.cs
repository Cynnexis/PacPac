﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PacPac
{
	public class SoundManager : AbstractSingleton
	{
		private static SoundManager instance = new SoundManager();

		private SoundEffect se_music;
		private SoundEffect se_menuMusic; // Music found on http://incompetech.com/music/royalty-free/
		private SoundEffect se_monsterEaten;
		private SoundEffect se_pacEaten;
		private SoundEffect se_pacEatPacdot0;
		private SoundEffect se_pacEatPacdot1;
		private SoundEffect se_invincible;

		private SoundEffectInstance sei_music;
		private SoundEffectInstance sei_menuMusic;
		private SoundEffectInstance sei_invincible;
		private bool toggle;

		public static SoundManager Instance
		{
			get { return instance; }
		}

		private SoundManager() : base()
		{
			toggle = false;
		}

		public void LoadContent(Game game)
		{
			se_music = game.Content.Load<SoundEffect>(@"Musics\Siren");
			se_menuMusic = game.Content.Load<SoundEffect>(@"Musics\PinballSpring");
			se_monsterEaten = game.Content.Load<SoundEffect>(@"Sounds\MonsterEaten");
			se_pacEaten = game.Content.Load<SoundEffect>(@"Sounds\PacmanEaten");
			se_pacEatPacdot0 = game.Content.Load<SoundEffect>(@"Sounds\PacmanEatPacdot0");
			se_pacEatPacdot1 = game.Content.Load<SoundEffect>(@"Sounds\PacmanEatPacdot1");
			se_invincible = game.Content.Load<SoundEffect>(@"Sounds\Invincible");

			IsInitialized = true;
		}

		public void PlayMusic()
		{
			if (IsInitialized)
			{
				if (sei_music != null)
					sei_music.Resume();
				else
				{
					sei_music = se_music.CreateInstance();
					sei_music = ProcessAndPlayMusic(sei_music);
				}
			}
		}

		public void PauseMusic()
		{
			if (IsInitialized && sei_music != null)
				sei_music.Pause();
		}

		public void StopMusic()
		{
			if (IsInitialized && sei_music != null)
				sei_music.Stop();
		}

		public void PlayMenuMusic()
		{
			if (IsInitialized)
			{
				if (sei_menuMusic != null)
					sei_menuMusic.Resume();
				else
				{
					sei_menuMusic = se_menuMusic.CreateInstance();
					sei_menuMusic = ProcessAndPlayMusic(sei_menuMusic);
				}
			}
		}

		public void PauseMenuMusic()
		{
			if (IsInitialized && sei_music != null)
				sei_menuMusic.Pause();
		}

		public void StopMenuMusic()
		{
			if (IsInitialized && sei_menuMusic != null)
				sei_menuMusic.Stop();
		}

		public void PlayMonsterEaten()
		{
			if (IsInitialized)
				ProcessAndPlaySound(se_monsterEaten.CreateInstance());
		}

		public void PlayPacEaten()
		{
			if (IsInitialized)
				ProcessAndPlaySound(se_pacEaten.CreateInstance());
		}

		public void PlayPacEatPacDot()
		{
			if (IsInitialized)
			{
				if (!toggle)
					ProcessAndPlaySound(se_pacEatPacdot0.CreateInstance());
				else
					ProcessAndPlaySound(se_pacEatPacdot1.CreateInstance());

				toggle = !toggle;
			}
		}

		public void PlayInvincible()
		{
			if (IsInitialized)
				ProcessAndPlaySound(se_invincible.CreateInstance());


			if (IsInitialized)
			{
				if (sei_invincible != null)
					sei_invincible.Resume();
				else
				{
					sei_invincible = se_invincible.CreateInstance();
					sei_invincible = ProcessAndPlayMusic(sei_invincible, 0.8f);
				}
			}
		}

		public void PauseInvincible()
		{
			if (IsInitialized && sei_invincible != null)
				sei_invincible.Pause();
		}

		public void StopInvincible()
		{
			if (IsInitialized && sei_invincible != null)
				sei_invincible.Stop();
		}

		private SoundEffectInstance ProcessAndPlaySound(SoundEffectInstance sei)
		{
			if (sei != null)
			{
				sei.Volume = 0.8f;
				sei.Play();
			}
			return sei;
		}

		private SoundEffectInstance ProcessAndPlayMusic(SoundEffectInstance sei, float volume = 0.5f)
		{
			if (sei != null)
			{
				sei.Volume = volume;
				sei.IsLooped = true;
				sei.Play();
			}
			return sei;
		}
	}
}
