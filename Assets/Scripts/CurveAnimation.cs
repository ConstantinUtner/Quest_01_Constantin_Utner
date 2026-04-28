using System.Collections;
using UnityEngine;

public class CurveAnimation : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float waitTime;

    [SerializeField]
    private Vector3 a;

    [SerializeField]
    private Vector3 b;

    [SerializeField]
    private AnimationCurve curve;

    private void Start()
    {
        this.StartCoroutine(this.Animate());
    }

    private IEnumerator Animate()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            var position = Vector3.Lerp(this.a, this.b, t);
            position.y += this.curve.Evaluate(t);
            this.transform.position = position;
            t += Time.deltaTime * this.speed;
            yield return null;
        }
        this.transform.position = this.b + Vector3.up * this.curve.Evaluate(1);
    }
}
