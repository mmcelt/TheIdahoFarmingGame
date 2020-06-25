using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TmDice25D
{
    [SerializeField]
    public class DiceRoll : MonoBehaviour
    {
        enum RollStarte{
            None=0,
            Roll,
            Stop,
        };
        [SerializeField] int m_pip=-1;
        [SerializeField] Text buttonTxt=null;
        [SerializeField] Vector2 rollVec = Vector2.one * 200f;
        [SerializeField] int m_eyeMin = 1;
        [SerializeField] int m_eyeMax = 6;
        public int pip { get { return m_pip; } }
        RollStarte state;
        Animator anm;
        Rigidbody2D rb;
        AudioSource ac;
        Vector3 defPos;
        bool toRoll;

        // Use this for initialization
        void Start()
        {
            anm = GetComponent<Animator>();
            ac = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            defPos = transform.position;
            toRoll = false;
            StartCoroutine(diceRollCo());
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ac.Play();
            if (rb.velocity.sqrMagnitude < 10f)
            {
                state = RollStarte.Stop;
            }
            
        }

        IEnumerator diceRollCo()
        {
            while (true)
            {
                buttonTxt.text = "Roll";
                rb.isKinematic = true;
                transform.position = defPos;
                state = RollStarte.Stop;
                m_pip = -1;
                Roll();
                while (!toRoll)
                {
                    yield return null;
                }
                state = RollStarte.Roll;
                rb.isKinematic = false;
                rb.velocity = Vector2.zero;
                rb.AddForce(rollVec);

                Roll();

                while (state == RollStarte.Roll)
                {
                    yield return null;
                }

                SetPip(Random.Range(m_eyeMin, m_eyeMax+1));

                buttonTxt.text = pip.ToString();
                toRoll = false;
                while (!toRoll)
                {
                    yield return null;
                }
                buttonTxt.text = "Roll";
            }
        }

        public void OnRollButton()
        {
            toRoll = true;
        }

        /// <summary>
        /// Start roll animation
        /// </summary>
        public void Roll()
        {
            anm.Play("roll");
        }

        /// <summary>
        /// Set pip and start pip animation
        /// </summary>
        /// <param name="_pip">pip of dice</param>
        public void SetPip(int _pip)
        {
            m_pip = _pip;
            anm.Play("to" + m_pip.ToString());
        }
    }
}
