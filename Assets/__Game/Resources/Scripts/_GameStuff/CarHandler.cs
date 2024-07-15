using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class CarHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
  {
    [Header("Audio")]
    [SerializeField] private AudioClip _honkCLip;

    public bool Placed { get; private set; } = false;

    private Vector3 _startLocalPosition;
    private Vector3 _originalPosition;
    private Vector3 _offset;
    private bool _canPlace = false;
    private bool _onCar = false;
    private bool _onRoad = false;

    private LevelContainer _levelContainer;

    private void Start() {
      StartCoroutine(DoMoveToAnotherParent());
    }

    private void OnTriggerEnter(Collider other) {
      if (other.TryGetComponent(out CarHandler carHandler)) {
        _onCar = true;
      }

      if (other.TryGetComponent(out RoadCollider roadCollider)) {
        _onRoad = true;
      }

      UpdateCanPlace();
    }

    private void OnTriggerExit(Collider other) {
      if (other.TryGetComponent(out CarHandler carHandler)) {
        _onCar = false;
      }

      if (other.TryGetComponent(out RoadCollider roadCollider)) {
        _onRoad = false;
      }

      UpdateCanPlace();
    }

    private IEnumerator DoMoveToAnotherParent() {
      yield return new WaitForEndOfFrame();

      transform.parent = transform.parent.parent;

      yield return new WaitForEndOfFrame();

      _startLocalPosition = transform.localPosition;

      _levelContainer = GetComponentInParent<LevelContainer>();
    }

    private void UpdateCanPlace() {
      _canPlace = _onRoad && !_onCar;
    }

    public void OnPointerDown(PointerEventData eventData) {
      Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      mouseWorldPos.z = transform.position.z;
      _offset = transform.position - mouseWorldPos;
      _originalPosition = transform.position;

      EventBus<EventStructs.VariantAudioClickedEvent>.Raise(
        new EventStructs.VariantAudioClickedEvent { AudioClip = _honkCLip });

      _levelContainer.SwitchTutorial(2);
    }

    public void OnPointerUp(PointerEventData eventData) {
      if (_canPlace == false) {
        transform.DOLocalMove(_startLocalPosition, 0.25f);

        Placed = false;
      }
      else {
        Placed = true;
      }

      EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent());
    }

    public void OnDrag(PointerEventData eventData) {
      Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      mouseWorldPos.z = transform.position.z;

      Vector3 newPosition = mouseWorldPos + _offset;

      transform.position = new Vector3(newPosition.x, newPosition.y, _originalPosition.z);
    }
  }
}