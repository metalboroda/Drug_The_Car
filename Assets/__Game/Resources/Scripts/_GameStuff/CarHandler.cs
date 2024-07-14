using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class CarHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
  {
    private Vector3 _originalPosition;
    private Vector3 _offset;

    public void OnPointerDown(PointerEventData eventData) {
      Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      mouseWorldPos.z = transform.position.z;
      _offset = transform.position - mouseWorldPos;
      _originalPosition = transform.position;
    }

    public void OnPointerUp(PointerEventData eventData) {
    }

    public void OnDrag(PointerEventData eventData) {
      Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

      mouseWorldPos.z = transform.position.z;

      Vector3 newPosition = mouseWorldPos + _offset;

      transform.position = new Vector3(newPosition.x, newPosition.y, _originalPosition.z);
    }
  }
}