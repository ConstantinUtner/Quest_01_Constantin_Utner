using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenedMovingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField]
    private List<Transform> waypoints;

    [SerializeField]
    private List<float> movingTimes;

    [SerializeField]
    private float waitingTime;

    private bool isPlaying = false;
    private Sequence sequence;

    private Vector3 lastPosition;
    private Vector3 currentVelocity;

    private void Start()
    {
        this.lastPosition = this.transform.position;

        this.StartCoroutine(this.TrackVelocity());
    }

    private IEnumerator TrackVelocity()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            this.currentVelocity =
                (this.transform.position - this.lastPosition) / Time.fixedDeltaTime;
            this.lastPosition = this.transform.position;
        }
    }

    public Vector3 GetVelocity()
    {
        return this.currentVelocity;
    }

    private void CreateSequence()
    {
        this.sequence = DOTween.Sequence();

        this.sequence.SetUpdate(UpdateType.Fixed);

        for (int i = 0; i < this.waypoints.Count; i++)
        {
            var tween = this.transform.DOMove(this.waypoints[i].position, this.movingTimes[i]);
            tween.SetEase(Ease.InOutQuint);
            this.sequence.Append(tween);
            this.sequence.AppendInterval(this.waitingTime);
        }

        for (int i = this.waypoints.Count - 2; i >= 1; i--)
        {
            var tween = this.transform.DOMove(this.waypoints[i].position, this.movingTimes[i]);
            tween.SetEase(Ease.InOutQuint);
            this.sequence.Append(tween);
            this.sequence.AppendInterval(this.waitingTime);
        }
    }

    private IEnumerator Play()
    {
        this.isPlaying = true;

        this.CreateSequence();
        this.sequence.Play();

        yield return this.sequence.WaitForCompletion();

        this.isPlaying = false;
    }

    private void Update()
    {
        if (!this.isPlaying)
        {
            this.StartCoroutine(this.Play());
        }
    }
}
