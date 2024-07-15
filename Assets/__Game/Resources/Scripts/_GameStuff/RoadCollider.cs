using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class RoadCollider : MonoBehaviour
  {
    public int CarsOnRoadCounter { get; private set; }

    private void OnTriggerEnter(Collider other) {
      if (other.TryGetComponent(out CarHandler carHandler))
        CarsOnRoadCounter++;
    }

    private void OnTriggerExit(Collider other) {
      if (other.TryGetComponent(out CarHandler carHandler)) {
        if (CarsOnRoadCounter > 0)
          CarsOnRoadCounter--;
      }
    }
  }
}