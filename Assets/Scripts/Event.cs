using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Event {
    public float time_start;
    public int tick, activate_tick, duration, variance;
    bool recurrent;
    public EventFunction eventFunction;
    public GameObject gameObject;
    public Event(GameObject gameObject_, int duration_, EventFunction eventFunction_, bool recurrent_ = false, int variance_ = 0) {
        tick = S_World.tick;
        duration = duration_;
        variance = variance_;
        activate_tick = tick + duration_ + (int)(variance*Random.value);
        eventFunction = eventFunction_;
        gameObject = gameObject_;
        recurrent = recurrent_;
    }

    // This is not a real Tick function as it gets called by the event handler.
    public bool Tick() {
        //Debug.Log("Event:Tick tick="+tick+" activate_tick="+activate_tick);
        tick += 1;
        if (tick == activate_tick) {
            if (gameObject != null)
                eventFunction();
            return true;
        }
        return false;
    }

    public bool IsRecurrent() {
        return recurrent;
    }

    public void Reschedule() {
        activate_tick = S_World.tick + duration + (int)(variance*Random.value);
    }

}

public delegate void EventFunction();