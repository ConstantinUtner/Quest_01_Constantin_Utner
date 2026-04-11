using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float platformSpeed;

    [SerializeField]
    private Vector3 start;

    [SerializeField]
    private Vector3 end;

    [SerializeField]
    private Lever activationLever;

    private Vector3 lastPosition;
    private float currentPathTime;

    void Start()
    {
        this.lastPosition = this.transform.position;
    }

    void FixedUpdate()
    {
        this.lastPosition = this.transform.position;

        if (this.activationLever == null || this.activationLever.IsOn)
        {
            this.currentPathTime += Time.fixedDeltaTime;
        }

        float pingPong = Mathf.PingPong(this.currentPathTime * this.platformSpeed, 1.0f);
        var newPosition = Vector3.Lerp(this.start, this.end, pingPong);
        this.transform.localPosition = newPosition;
    }

    public Vector3 GetVelocity()
    {
        return (this.transform.position - this.lastPosition) / Time.fixedDeltaTime;
    }
}
