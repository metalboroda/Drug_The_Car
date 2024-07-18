using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static __Game.Resources.Scripts.EventBus.EventStructs;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class LevelContainer : MonoBehaviour
  {
    [Header("Logic")]
    [SerializeField] private int _carsAmountToWin = 1;
    [Header("Car Spawning")]
    [SerializeField] private List<GameObject> _carsToSpawn = new List<GameObject>();
    [Space]
    [Range(0, 5)]
    [SerializeField] private int _spawnCarsAmount;
    [Space]
    [SerializeField] private Transform _carsContainer;
    [Header("UI")]
    [SerializeField] private Button _voiceButton;
    [SerializeField] private Button _submitButton;
    [Header("Audio")]
    [SerializeField] private AudioClip _voiceClip;
    [SerializeField] private AudioClip _carEngineClip;
    [Header("Tutorial")]
    [SerializeField] private bool _tutorial;
    [Space]
    [SerializeField] private GameObject[] _tutorialFingers;

    private Coroutine _stuporTimeoutRoutine;

    private GameBootstrapper _gameBootstrapper;
    private RoadCollider _roadCollider;

    private EventBinding<EventStructs.StateChanged> _stateChangedEvent;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;
      _roadCollider = GetComponentInChildren<RoadCollider>();

      _submitButton.gameObject.SetActive(false);

      SpawnRandomCars();
    }

    private void OnEnable() {
      _stateChangedEvent = new EventBinding<StateChanged>(StuporTimerDependsOnState);

      SubscribeButtons();
    }

    private void Start() {
      _stateChangedEvent.Remove(StuporTimerDependsOnState);

      SwitchTutorial(0);
      ResetAndStartStuporTimer();

      EventBus<VariantsAssignedEvent>.Raise(new EventStructs.VariantsAssignedEvent());
    }

    private void SubscribeButtons() {
      _voiceButton.onClick.AddListener(() => {
        _submitButton.gameObject.SetActive(true);

        EventBus<EventStructs.VariantAudioClickedEvent>.Raise(
            new EventStructs.VariantAudioClickedEvent { AudioClip = _voiceClip });

        SwitchTutorial(1);
        ResetAndStartStuporTimer();
      });

      _submitButton.onClick.AddListener(() => {
        CheckForWin();
        SwitchTutorial(3);
      });
    }

    private void SpawnRandomCars() {
      if (_spawnCarsAmount == 0) return;

      for (int i = 0; i < _spawnCarsAmount; i++) {
        Instantiate(_carsToSpawn[Random.Range(0, _carsToSpawn.Count)], _carsContainer);
      }
    }

    private void CheckForWin() {
      if (_roadCollider.CarsOnRoadCounter == _carsAmountToWin) {
        EventBus<EventStructs.Win>.Raise(new EventStructs.Win());

        if (_gameBootstrapper != null) {
          _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1.5f, this);

          EventBus<EventStructs.VariantAudioClickedEvent>.Raise(
            new EventStructs.VariantAudioClickedEvent { AudioClip = _carEngineClip });
        }
      }
      else {
        EventBus<EventStructs.Lose>.Raise(new EventStructs.Lose());

        if (_gameBootstrapper != null) {
          _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameLoseState(_gameBootstrapper), 0.25f, this);
        }
      }
    }

    public void SwitchTutorial(int index) {
      foreach (var tutorialFinger in _tutorialFingers) {
        tutorialFinger.SetActive(false);
      }

      if (_tutorial == false) return;

      if (index > _tutorialFingers.Length - 1) {
        foreach (var tutorialFinger in _tutorialFingers) {
          tutorialFinger.SetActive(false);
        }
      }
      else {
        _tutorialFingers[index].SetActive(true);
      }
    }

    private void StuporTimerDependsOnState(StateChanged stateChanged) {
      if (stateChanged.State is GameplayState)
        ResetAndStartStuporTimer();
      else {
        if (_stuporTimeoutRoutine != null)
          StopCoroutine(_stuporTimeoutRoutine);
      }
    }

    public void ResetAndStartStuporTimer() {
      if (_stuporTimeoutRoutine != null)
        StopCoroutine(_stuporTimeoutRoutine);

      _stuporTimeoutRoutine = StartCoroutine(DoStuporTimerCoroutine());
    }

    private IEnumerator DoStuporTimerCoroutine() {
      yield return new WaitForSeconds(15);

      EventBus<StuporEvent>.Raise(new StuporEvent());

      ResetAndStartStuporTimer();
    }
  }
}