using System.Collections;
using UnityEngine;

public class SmoothStepTween : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float waitTime;

    [SerializeField]
    private Vector3 a;

    [SerializeField]
    private Vector3 b;

    private void Start()
    {
        this.StartCoroutine(this.SmoothMove());
    }

    private IEnumerator SmoothMove()
    {
        yield return new WaitForSeconds(this.waitTime);
        yield return this.StartCoroutine(this.SmoothStepForward());
        yield return new WaitForSeconds(this.waitTime);
        yield return this.StartCoroutine(this.SmoothStepBackward());
    }

    private IEnumerator SmoothStepForward()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            float g = Mathf.SmoothStep(0.0f, 1.0f, t);
            this.transform.position = Vector3.Lerp(this.a, this.b, g);
            t += Time.deltaTime * this.speed;
            yield return null;
        }
        this.transform.position = this.b;
    }

    private IEnumerator SmoothStepBackward()
    {
        float t = 1.0f;
        while (t > 0.0f)
        {
            float g = Mathf.SmoothStep(0.0f, 1.0f, t);
            this.transform.position = Vector3.Lerp(this.a, this.b, g);
            t -= Time.deltaTime * this.speed;
            yield return null;
        }
        this.transform.position = this.a;
    }
}
