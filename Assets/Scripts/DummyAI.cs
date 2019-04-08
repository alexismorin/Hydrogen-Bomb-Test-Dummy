using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class DummyAI : MonoBehaviour {
    Animator anim;
    NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    public float wanderRadius;
    public float wanderTimer;

    private Transform target;

    private float timer;

    void Start () {
        wanderTimer = Random.Range (3f, 7f);

        anim = GetComponent<Animator> ();
        agent = GetComponent<NavMeshAgent> ();
        // Don’t update position automatically
        agent.updatePosition = false;

        InvokeRepeating ("SetNewDestination", 0.5f, wanderTimer);
    }

    void Update () {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot (transform.right, worldDeltaPosition);
        float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min (1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        // Update animation parameters
        anim.SetBool ("move", shouldMove);
        anim.SetFloat ("velx", velocity.x);
        anim.SetFloat ("vely", velocity.y);

        //      GetComponent<LookAt> ().lookAtTargetPosition = agent.steeringTarget + transform.forward;

    }

    public void SetNewDestination () {
        Vector3 newPos = RandomNavSphere (transform.position, wanderRadius, -1);

        if (agent.isOnNavMesh) {
            agent.SetDestination (newPos);
        }

    }

    void OnAnimatorMove () {
        // Update position to agent position
        transform.position = agent.nextPosition;
    }

    void OnEnable () {

    }

    public static Vector3 RandomNavSphere (Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag == "piece") {
            if (collision.relativeVelocity.magnitude > 2f) {
                anim.applyRootMotion = false;
                Invoke ("Kill", 0.01f);
            }

        }
    }

    public void Kill () {
        Destroy (this);
        agent.updatePosition = true;

        if (agent.destination != null && agent.isOnNavMesh) {
            agent.ResetPath ();
        }
        agent.enabled = false;
        anim.enabled = false;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody> ();

        foreach (Rigidbody rb in rigidbodies) {
            rb.isKinematic = false;
        }

    }

}