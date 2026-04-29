using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum MovementAxis
    {
        LeftRight,
        ForwardBack,
    }

    [Header("Movement Settings")]
    [SerializeField]
    private MovementAxis moveAxis = MovementAxis.LeftRight;

    [SerializeField]
    private float speed = 2.0f;

    [SerializeField]
    private float patrolDistance = 3.0f;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip squashSound;

    private Vector3 startPosition;
    private bool movingPositive = true;
    private bool isDead = false;
    private Animator animator;

    void Start()
    {
        this.startPosition = this.transform.position;
        this.animator = this.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (this.isDead)
            return;

        if (this.moveAxis == MovementAxis.LeftRight)
        {
            float leftEdge = this.startPosition.x - this.patrolDistance;
            float rightEdge = this.startPosition.x + this.patrolDistance;

            if (this.movingPositive)
            {
                this.transform.Translate(Vector3.right * this.speed * Time.deltaTime, Space.World);
                this.transform.rotation = Quaternion.LookRotation(Vector3.right);
                if (this.transform.position.x >= rightEdge)
                    this.movingPositive = false;
            }
            else
            {
                this.transform.Translate(Vector3.left * this.speed * Time.deltaTime, Space.World);
                this.transform.rotation = Quaternion.LookRotation(Vector3.left);
                if (this.transform.position.x <= leftEdge)
                    this.movingPositive = true;
            }
        }
        else
        {
            float backEdge = this.startPosition.z - this.patrolDistance;
            float forwardEdge = this.startPosition.z + this.patrolDistance;

            if (this.movingPositive)
            {
                this.transform.Translate(
                    Vector3.forward * this.speed * Time.deltaTime,
                    Space.World
                );
                this.transform.rotation = Quaternion.LookRotation(Vector3.forward);
                if (this.transform.position.z >= forwardEdge)
                    this.movingPositive = false;
            }
            else
            {
                this.transform.Translate(Vector3.back * this.speed * Time.deltaTime, Space.World);
                this.transform.rotation = Quaternion.LookRotation(Vector3.back);
                if (this.transform.position.z <= backEdge)
                    this.movingPositive = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.isDead)
            return;

        Character player = other.GetComponent<Character>();

        if (player != null)
        {
            if (player.transform.position.y > this.transform.position.y + 0.2f)
            {
                player.EnemyBounce();
                this.SquashAndDie();
            }
            else
            {
                player.Respawn();
            }
        }
    }

    private void SquashAndDie()
    {
        this.isDead = true;

        if (this.audioSource != null && this.squashSound != null)
        {
            this.audioSource.PlayOneShot(this.squashSound);
        }

        if (this.animator != null)
        {
            this.animator.speed = 0;
        }

        this.transform.DOScale(new Vector3(1.5f, 0.1f, 1.5f), 0.2f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                Destroy(this.gameObject, 1.0f);
            });
    }
}
