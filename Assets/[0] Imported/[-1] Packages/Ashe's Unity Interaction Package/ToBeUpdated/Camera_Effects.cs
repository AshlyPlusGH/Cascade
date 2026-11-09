using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class Camera_Effects : MonoBehaviour
{
    [SerializeField] private bool debug;
        private void Log(string contents){ if (debug){ Debug.Log(contents + " at: " + name); }}

    [Space(10)]

    [SerializeField] private Camera target;

    void Awake(){ Setup(); }
    public void Setup(){  }

    [Button] public void Shake(float duration = 1, float shakeMagnitude = 1.0f){ StartCoroutine(Action_Shake(duration, shakeMagnitude)); } //For a Non Transforming Camera Only
        IEnumerator Action_Shake(float duration, float shakeMagnitude = 1.0f)
        {
            Vector3 startPosition = target.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                target.transform.position = startPosition + (Random.insideUnitSphere * shakeMagnitude);
                yield return null;
            }

            target.transform.position = startPosition;
        }
}