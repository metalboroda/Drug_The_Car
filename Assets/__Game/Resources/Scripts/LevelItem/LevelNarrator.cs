﻿using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using Assets.__Game.Scripts.Tools;
using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.LevelItem
{
  public class LevelNarrator : MonoBehaviour
  {
    [Header("Announcer")]
    [SerializeField] private AudioClip _questStartClip;
    [SerializeField] private AudioClip[] _questClips;
    [Space]
    [SerializeField] private float _delayBetweenClips = 0.25f;
    [Space]
    [SerializeField] private AudioClip[] _winAnnouncerClips;
    [SerializeField] private AudioClip[] _loseAnnouncerClips;
    [SerializeField] private AudioClip[] _stuporAnnouncerClips;

    private bool _questClipsArePlayed;

    private AudioSource _audioSource;

    private GameBootstrapper _gameBootstrapper;
    private AudioTool _audioTool;

    private EventBinding<EventStructs.StateChanged> _stateEvent;
    private EventBinding<EventStructs.StuporEvent> _stuporEvent;
    private EventBinding<EventStructs.UiButtonEvent> _uiButtonEvent;
    private EventBinding<EventStructs.VariantAudioClickedEvent> _variantAudioClickedEvent;
    private EventBinding<EventStructs.VoiceButtonAudioEvent> _voiceButtonAudioEvent;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;
      _audioTool = new AudioTool(_audioSource);

      _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
      _stateEvent = new EventBinding<EventStructs.StateChanged>(PlayScreenSound);
      _stuporEvent = new EventBinding<EventStructs.StuporEvent>(PlayStuporSound);
      //_uiButtonEvent = new EventBinding<EventStructs.UiButtonEvent>(PlayQuestClipsSequentially);
      _variantAudioClickedEvent = new EventBinding<EventStructs.VariantAudioClickedEvent>(PlayWordAudioCLip);
      _voiceButtonAudioEvent = new EventBinding<EventStructs.VoiceButtonAudioEvent>(PlayButtonVoice);
    }

    private void OnDisable() {
      _stateEvent.Remove(PlayScreenSound);
      _stuporEvent.Remove(PlayStuporSound);
      //_uiButtonEvent.Remove(PlayQuestClipsSequentially);
      _variantAudioClickedEvent.Remove(PlayWordAudioCLip);
      _voiceButtonAudioEvent.Remove(PlayButtonVoice);
    }

    private void Start() {
      if (_questStartClip != null && _gameBootstrapper.StateMachine.CurrentState is GameQuestState)
        _audioSource.PlayOneShot(_questStartClip);
    }

    private void PlayScreenSound(EventStructs.StateChanged state) {
      switch (state.State) {
        case GameWinState:
          _audioSource.Stop();
          _audioSource.PlayOneShot(_audioTool.GetRandomCLip(_winAnnouncerClips));
          break;
        case GameLoseState:
          _audioSource.Stop();
          _audioSource.PlayOneShot(_audioTool.GetRandomCLip(_loseAnnouncerClips));
          break;
      }
    }

    private void PlayButtonVoice(EventStructs.VoiceButtonAudioEvent voiceButtonAudioEvent) {
      _audioSource.Stop();
      _audioSource.PlayOneShot(voiceButtonAudioEvent.AudioClip);
    }

    private void PlayStuporSound(EventStructs.StuporEvent stuporEvent) {
      _audioSource.Stop();
      _audioSource.PlayOneShot(_audioTool.GetRandomCLip(_stuporAnnouncerClips));
    }

    public void PlayQuestClipsSequentially(EventStructs.UiButtonEvent uiButtonEvent) {
      if (_questClipsArePlayed == true) return;

      _questClipsArePlayed = true;

      if (uiButtonEvent.UiEnums == __Game.Scripts.Enums.UiEnums.QuestPlayButton) {
        if (_questClips.Length > 0)
          StartCoroutine(DoPlayClipsSequentially(_questClips));
      }
    }

    private IEnumerator DoPlayClipsSequentially(AudioClip[] clips) {
      if (_questClips.Length == 0) yield break;

      yield return new WaitForSecondsRealtime(0.1f);

      foreach (var clip in clips) {
        _audioSource.Stop();
        _audioSource.PlayOneShot(clip);

        yield return new WaitForSecondsRealtime(clip.length + _delayBetweenClips);
      }
    }

    private void PlayWordAudioCLip(EventStructs.VariantAudioClickedEvent variantAudioClickedEvent) {
      if (variantAudioClickedEvent.AudioClip != null) {
        _audioSource.Stop();
        _audioSource.PlayOneShot(variantAudioClickedEvent.AudioClip);
      }
    }
  }
}