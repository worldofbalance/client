using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SD
{
    public class BuffBackend : MonoBehaviour
    {
        // Enumerator to make buff type more understandable.
        public enum BuffMechanism : int { MULTIPLICITIVE, ADDITIVE, FLAT };

        // The base/reference stat, and the buffed amount.
        private float baseStat = 0;
        private float adjustedStat = 0;

        // The absolute maximum that this stat can be boosted to.
        private float maxBuffEffect = float.PositiveInfinity;

        // Multiplier per stack and current multiplier amount.
        private float buffBonusPerStack = 1.0f;
        private float currentBuffBonus = 1.0f;
        private float initialBuffBonus = 1.0f;

        // The type of buff mechanism. Multiplicitive, additive, or flat.
        private BuffMechanism buffMechanism = BuffMechanism.MULTIPLICITIVE;

        // The current and max stacks for this buff.
        private int buffStackAmount = 0;
        private int maxBuffStackAmount = 1;

        // The end time and max duration of the buff.
        private float buffEndTime = 0.0f;
        private float buffMaxDuration = 0.0f;

        void Start()
        {
            Console.WriteLine("Buff Template Awake.");
        }

        // Stat updates take place in LateUpdate to allow the extended buff
        // to continue using the regular Update function.
        private void LateUpdate()
        {
            // Update and reset base stat and multiplier if buff duration has elapsed.
            if (Time.timeSinceLevelLoad > buffEndTime)
            {
                adjustedStat = baseStat;
                currentBuffBonus = initialBuffBonus;
            }
        }

        /// <summary>
        /// Add a stack of the buff, if possible.
        /// </summary>
        /// <returns>Returns the recalculated stat value.</returns>
        public float AddStack()
        {
            // Check for too many stacks. Add a stack if ok.
            if (buffStackAmount < maxBuffStackAmount)
            {
                buffStackAmount++;
                // After adding a stack, recalculate the adjustedStat value.
                // case 0: multiplicitive multiplier.
                // case 1: additive multiplier.
                // case 2: flat bonus.
                switch (buffMechanism)
                {
                    case BuffMechanism.MULTIPLICITIVE:
                        {
                            currentBuffBonus = currentBuffBonus * buffBonusPerStack;
                            adjustedStat = baseStat * currentBuffBonus;
                            break;
                        }
                    case BuffMechanism.ADDITIVE:
                        {
                            currentBuffBonus = currentBuffBonus + buffBonusPerStack;
                            adjustedStat = baseStat * currentBuffBonus;
                            break;
                        }
                    case BuffMechanism.FLAT:
                        {
                            currentBuffBonus = currentBuffBonus + buffBonusPerStack;
                            adjustedStat = baseStat + currentBuffBonus;
                            break;
                        }
                    default:
                        break;
                }

                // Update the end time of the buff.
                buffEndTime = Time.timeSinceLevelLoad + buffMaxDuration;

                // Clamp it to the maximum stat value provided if it goes over.
                adjustedStat = (adjustedStat <= maxBuffEffect) ? adjustedStat : maxBuffEffect;
            }

            // Finally, return the adjusted stat value.
            return adjustedStat;
        }


        #region Getters and Setters

        /// <summary>
        /// Getter for the adjusted stat value.
        /// </summary>
        /// <returns></returns>
        public float GetAdjustedStatAmount()
        {
            return adjustedStat;
        }

        /// <summary>
        /// Setter for the base value of the stat the buff will affect.
        /// </summary>
        /// <param name="baseStat"></param>
        public void SetBaseStat(float baseStat)
        {
            this.baseStat = adjustedStat = baseStat;
        }

        /// <summary>
        /// Setter for the max amount of buff stacks receivable.
        /// </summary>
        /// <param name="maxStacks"></param>
        public void SetMaxStackAmount(int maxStacks)
        {
            this.maxBuffStackAmount = maxStacks;
        }

        /// <summary>
        /// Setter for the buff multiplier per stack.
        /// </summary>
        /// <param name="multiplier"></param>
        public void SetBuffBonusPerStack(float multiplier)
        {
            buffBonusPerStack = multiplier;
        }

        /// <summary>
        /// Sets the buff mechanism, either additive or multiplicitive.
        /// 0 = multiplicitive, 1 = additive.
        /// For example:
        /// s2 stacks of multiplicitive buff with 1.1x multiplier per stack = 1.1 * 1.1 * base.
        /// 2 stacks of additive buff with 1.1x multiplier = 1.1 + 1.1 * base.
        /// </summary>
        /// <param name="mechanism"></param>
        public void SetBuffMechanism(BuffMechanism mechanism)
        {
            // Clamp number between [0, 1] inclusive.
            buffMechanism = mechanism;
        }

        /// <summary>
        /// Setter for the maximum buff duration in seconds.
        /// </summary>
        /// <param name="duration"></param>
        public void SetMaxBuffDuration(float duration)
        {
            buffMaxDuration = duration;
        }

        /// <summary>
        /// Returns the number of stacks of the current buff.
        /// </summary>
        /// <returns></returns>
        public int GetBuffStackAmount()
        {
            return buffStackAmount;
        }

        /// <summary>
        /// Sets the maximum that the stat can grow to, regardless of further stacks.
        /// </summary>
        /// <param name="maxStatAmount"></param>
        public void SetAbsoluteMaxStatAmount(float maxStatAmount)
        {
            maxBuffEffect = maxStatAmount;
        }

        public void SetInitialBuffBonus(float newAmount)
        {
            currentBuffBonus = initialBuffBonus = newAmount;
        }

        #endregion
    }
}