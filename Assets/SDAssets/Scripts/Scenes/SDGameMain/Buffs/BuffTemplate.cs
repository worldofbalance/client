using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTemplate : MonoBehaviour
{
    // The base/reference stat, and the buffed amount.
    private float baseStat;
    private float adjustedStat;

    // Multiplier per stack and current multiplier amount.
    private float multiplierPerBuffStack = 1.0f;
    private float currentMultiplier = 1.0f;

    // The type of buff mechanism. 0 = multiplicitive, 1 = additive.
    private int buffMechanism = 0;

    // The current and max stacks for this buff.
    private int buffStackAmount = 0;
    private int maxBuffStackAmount = 1;

    // The end time and max duration of the buff.
    private float buffEndTime = 0.0f;
    private float buffMaxDuration = 0.0f;

    // Update the value per-frame.
	void Update ()
    {
        // Update and reset base stat and multiplier if buff duration has elapsed.
		if(Time.timeSinceLevelLoad > buffEndTime)
        {
            adjustedStat = baseStat;
            currentMultiplier = 1.0f;
        }
	}

    /// <summary>
    /// Add a stack of the buff, if possible.
    /// </summary>
    /// <returns>Returns the recalculated stat value.</returns>
    public float AddStack()
    {
        // Check for too many stacks. Add a stack if ok.
        if(buffStackAmount < maxBuffStackAmount)
        {
            buffStackAmount++;
            if(buffMechanism == 0)
            {
                currentMultiplier = currentMultiplier * multiplierPerBuffStack;
            }
            else if(buffMechanism == 1)
            {
                currentMultiplier = currentMultiplier + multiplierPerBuffStack;
            }

            // Update the end time of the buff.
            buffEndTime = Time.timeSinceLevelLoad + buffMaxDuration;

            // After adding a stack, recalculate the buffedStat value.
            adjustedStat = baseStat * currentMultiplier;
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
        this.baseStat = baseStat;
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
    public void SetMultiplerPerBuffStack(float multiplier)
    {
        multiplierPerBuffStack = multiplier;
    }

    /// <summary>
    /// Sets the buff mechanism, either additive or multiplicitive.
    /// 0 = multiplicitive, 1 = additive.
    /// For example:
    /// s2 stacks of multiplicitive buff with 1.1x multiplier per stack = 1.1 * 1.1 * base.
    /// 2 stacks of additive buff with 1.1x multiplier = 1.1 + 1.1 * base.
    /// </summary>
    /// <param name="mechanism"></param>
    public void SetBuffMechanism(int mechanism)
    {
        // Clamp number between [0, 1].
        buffMechanism = (mechanism < 0) ? 0 : (mechanism > 1) ? 1 : mechanism;
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

    #endregion
}
