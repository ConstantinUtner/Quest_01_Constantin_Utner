using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float platformSpeed;

    [SerializeField]
    private Vector3 start;

    [SerializeField]
    private Vector3 end;

    private Vector3 lastPosition;

    void Start()
    {
        this.lastPosition = this.transform.position;
    }

    void FixedUpdate()
    {
        this.lastPosition = this.transform.position;
        float pingPong = Mathf.PingPong(Time.fixedTime * this.platformSpeed, 1.0f);
        var newPosition = Vector3.Lerp(this.start, this.end, pingPong);
        this.transform.localPosition = newPosition;
    }

    public Vector3 GetVelocity()
    {
        return (this.transform.position - this.lastPosition) / Time.fixedDeltaTime;
    }
}
