using System.Collections.Generic;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class CarWheelsHandler : MonoBehaviour
  {
    private List<GameObject> _wheels = new List<GameObject>();

    void Start() {
      _wheels = GetAllWheelObjects();
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
  }
}