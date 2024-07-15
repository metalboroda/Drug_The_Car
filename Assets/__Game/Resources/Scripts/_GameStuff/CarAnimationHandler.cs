using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class CarAnimationHandler : MonoBehaviour
  {
    [Header("Car")]
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _movementDestinationX = 250f;
    [Header("Wheels")]
    [SerializeField] private float _wheelRotationSpeed = 100f;

    private List<GameObject> _wheels = new List<GameObject>();

    private CarHandler _carHandler;

    private EventBinding<EventStructs.Win> _winEvent;

    private void Awake() {
      _carHandler = GetComponent<CarHandler>();
    }

    private void OnEnable() {
      _winEvent = new EventBinding<EventStructs.Win>(MoveCar);
      _winEvent = new EventBinding<EventStructs.Win>(RotateWheels);
    }

    private void OnDisable() {
      _winEvent.Remove(MoveCar);
      _winEvent.Remove(RotateWheels);
    }

    void Start() {
      _wheels = GetAllWheelObjects();
    }

    private void OnDestroy() {
      DOTween.Kill(transform);

      foreach (var wheel in _wheels) {
        DOTween.Kill(wheel.transform);
      }
    }

    List<GameObject> GetAllWheelObjects() {
      List<GameObject> wheelObjects = new List<GameObject>();

      GetAllWheelObjectsRecursive(transform, wheelObjects);

      return wheelObjects;
    }

    void GetAllWheelObjectsRecursive(Transform parent, List<GameObject> wheelObjects) {
      foreach (Transform child in parent) {
        if (ContainsWord(child.gameObject.name, "Wheel") || ContainsWord(child.gameObject.name, "wheel")) {
          wheelObjects.Add(child.gameObject);
        }

        GetAllWheelObjectsRecursive(child, wheelObjects);
      }
    }

    bool ContainsWord(string name, string word) {
      char[] delimiters = { ' ', '_', '-', '.', ',' };
      string[] words = name.Split(delimiters);

      foreach (string w in words) {
        if (w.Equals(word, System.StringComparison.OrdinalIgnoreCase)) {
          return true;
        }
      }
      return false;
    }

    private void MoveCar(EventStructs.Win win) {
      if (_carHandler.Placed == false) return;

      try {
        transform.DOLocalMoveX(_movementDestinationX, _movementSpeed)
          .SetSpeedBased(true);
      }
      catch {
      }

    }

    private void RotateWheels(EventStructs.Win win) {
      if (_carHandler.Placed == false) return;

      Vector3 rotationDirection = new Vector3(0, 0, -360);

      try {
        foreach (var wheel in _wheels) {
          wheel.transform.DOLocalRotate(rotationDirection, _wheelRotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetSpeedBased(true);
        }
      }
      catch {
      }
    }
  }
}