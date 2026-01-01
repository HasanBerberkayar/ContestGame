using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    PlayerBehavior player;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();   
    }
    void Start()
    {
        player = FindAnyObjectByType<PlayerBehavior>();
       
    }

    void Update()
    {
         agent.SetDestination(player.transform.position);
    }
}
