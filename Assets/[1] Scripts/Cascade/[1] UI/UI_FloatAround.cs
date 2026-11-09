using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UI_FloatAround : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;

    [SerializeField] private Vector2 xConstraints = new(10,10);
    [SerializeField] private Vector2 yConstraints = new(10,10);
    [SerializeField] private float speed = 25f;

    [field:SerializeField,ReadOnly] private Vector2 target;

    private void Awake()
    {
            if (targetRect == null) targetRect = GetComponent<RectTransform>();
        PickNewTarget();
    }

    private void Update()
    {
        targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, target, speed * Time.deltaTime);

        if (Vector2.Distance(targetRect.anchoredPosition, target) < 1f) PickNewTarget();
    }

    private void PickNewTarget()
    {
        target = new Vector2(
            Random.Range(xConstraints.x, xConstraints.y),
            Random.Range(yConstraints.x, yConstraints.y)
        );
    }
}