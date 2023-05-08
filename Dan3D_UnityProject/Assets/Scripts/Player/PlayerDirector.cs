using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player 
{
    public class PlayerDirector : MonoBehaviour
    {
        public enum PlayerStates {
            Idle,
            Walking,
            Interacting,
            Attacking
        }

        public static PlayerStates PlayerState = PlayerStates.Idle;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
