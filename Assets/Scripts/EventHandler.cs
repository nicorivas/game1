using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EventHandler {
    List<Event> events;
    
    public EventHandler() {
        S_World.OnTick += Tick;
        events = new List<Event>();
    }

    public void Add(Event e) {
        events.Add(e);
    }

    public void RemoveFromGameObject(GameObject g) {
        events.RemoveAll(x => x.gameObject == g);
    }

    public void RemoveFromGameObjectAndFunction(GameObject g, EventFunction f) {
        events.RemoveAll(x => x.gameObject == g && x.eventFunction == f);
    }

    public void Tick(object sender, S_World.OnTickEventArgs e) {
        foreach (Event ev in events.Reverse<Event>()) {
            if (ev.Tick()) {
                if (ev.IsRecurrent()) {
                    ev.Reschedule(); 
                } else {
                    events.Remove(ev);
                }
            }
        }
    }
}