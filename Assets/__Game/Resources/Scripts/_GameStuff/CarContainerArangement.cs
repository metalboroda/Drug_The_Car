using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class CarContainerArangement : MonoBehaviour
  {
    [SerializeField] private float _spacing = 2f;

    private void Start() {
      StartCoroutine(DoDisableArangement());
    }

    private void Update() {
      ArrangeHorizontally();
    }

    private void ArrangeHorizontally() {
      int childCount = transform.childCount;

      if (childCount == 0) return;

      float totalWidth = (childCount - 1) * _spacing;

      for (int i = 0; i < childCount; i++) {
        Transform child = transform.GetChild(i);

        float xPos = -totalWidth / 2 + i * _spacing;

        child.localPosition = new Vector3(xPos, 0, 0);
      }
    }

    private IEnumerator DoDisableArangement() {
      yield return new WaitForSeconds(1);

      this.enabled = false;
    }
  }
}