using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PacPac
{
	/// <summary>
	/// SoundManager is a singleton-class to manage the sound and music in the game
	/// </summary>
	public class SoundManager : AbstractSingleton
	{
		#region Attributes, Instance & Properties
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
		private bool toggle; // Indicate which sound play when pac eat a pacdot

		/// <summary>
		/// Unique instance of the class
		/// </summary>
		public static SoundManager Instance
		{
			get { return instance; }
			private set { instance = value; }
		}
		#endregion

		#region Constructor & Load Content
		/// <summary>
		/// Default constructor
		/// </summary>
		private SoundManager() : base()
		{
			// Initalize pacdot toggle
			toggle = false;
		}

		/// <summary>
		/// Load every sound and music from the resources of the game.
		/// This method MUST BE called before using this method.
		/// </summary>
		/// <param name="game">The game instance, different from <c>null</c>, to load all contents</param>
		/// <exception cref="ArgumentNullException">Throw when <paramref name="game"/> is null</exception>
		public void LoadContent(Game game)
		{
			if (game == null)
				throw new ArgumentNullException();

			se_music = game.Content.Load<SoundEffect>(@"Musics\Siren");
			se_menuMusic = game.Content.Load<SoundEffect>(@"Musics\PinballSpring");
			se_monsterEaten = game.Content.Load<SoundEffect>(@"Sounds\MonsterEaten");
			se_pacEaten = game.Content.Load<SoundEffect>(@"Sounds\PacmanEaten");
			se_pacEatPacdot0 = game.Content.Load<SoundEffect>(@"Sounds\PacmanEatPacdot0");
			se_pacEatPacdot1 = game.Content.Load<SoundEffect>(@"Sounds\PacmanEatPacdot1");
			se_invincible = game.Content.Load<SoundEffect>(@"Sounds\Invincible");

			IsInitialized = true;
		}
		#endregion

		#region Musics & Sounds Region
		/// <summary>
		/// Play or resume the main music of the game. If it is already played, do nothing.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
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
			else
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");
		}

		/// <summary>
		/// Pause the main music of the game. If it is already paused, do nothing.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void PauseMusic()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			if (sei_music != null)
				sei_music.Pause();
		}

		/// <summary>
		/// Stop the main music of the game. If it is already stopped, do nothing.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void StopMusic()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			if (sei_music != null)
				sei_music.Stop();
		}

		/// <summary>
		/// Play or resume the main music of the main screen. If it is already played, do nothing.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
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
			else
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");
		}

		/// <summary>
		/// Pause the main music of the main screen. If it is already paused, do nothing.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void PauseMenuMusic()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			if (sei_music != null)
				sei_menuMusic.Pause();
		}

		/// <summary>
		/// Stop the main music of the main screen. If it is already stopped, do nothing.
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void StopMenuMusic()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			if (sei_menuMusic != null)
				sei_menuMusic.Stop();
		}

		/// <summary>
		/// Play the sound of a monster being eaten
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void PlayMonsterEaten()
		{
			if (IsInitialized)
				ProcessAndPlaySound(se_monsterEaten.CreateInstance());
			else
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");
		}

		/// <summary>
		/// Play the sound of pac being eaten
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void PlayPacEaten()
		{
			if (IsInitialized)
				ProcessAndPlaySound(se_pacEaten.CreateInstance());
			else
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");
		}

		/// <summary>
		/// Play the sound of pac eating a pacdot
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
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
			else
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");
		}

		/// <summary>
		/// Play or resume the music of invincibility for pac. If it is already played, do nothing
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void PlayInvincible()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			ProcessAndPlaySound(se_invincible.CreateInstance());
			
			if (sei_invincible != null)
				sei_invincible.Resume();
			else
			{
				sei_invincible = se_invincible.CreateInstance();
				sei_invincible = ProcessAndPlayMusic(sei_invincible, 0.8f);
			}
		}

		/// <summary>
		/// Pause the music of invincibility for pac. If it is already paused, do nothing
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void PauseInvincible()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			if (sei_invincible != null)
				sei_invincible.Pause();
		}

		/// <summary>
		/// Stop the music of invincibility for pac. If it is already stopped, do nothing
		/// </summary>
		/// <exception cref="InvalidOperationException">Throw if <see cref="LoadContent(Game)"/> has not been called beforehand</exception>
		public void StopInvincible()
		{
			if (!IsInitialized)
				throw new InvalidOperationException("SoundManager is not initialized yet. Please use SoundManager.LoadContent(Game) beforehand.");

			if (sei_invincible != null)
				sei_invincible.Stop();
		}
		#endregion

		#region Processing Region
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
		#endregion
	}
}
