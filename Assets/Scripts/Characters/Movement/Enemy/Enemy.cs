using UnityEngine;

namespace CrossoverWorlds.Characters.Movement.Enemy
{
    public class Enemy : MonoBehaviour
    {

        [SerializeField] private Transform playerTransform;
        [SerializeField] private int speed = 5;


        void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            Vector3 relativePos = playerTransform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);

            Quaternion current = transform.localRotation;

            transform.localRotation = Quaternion.Slerp(current, 
                                                                                    rotation, 
                                                                                    Time.deltaTime * speed);
        }

    }
}
