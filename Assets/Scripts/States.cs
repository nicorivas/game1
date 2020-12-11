using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public int state;
    public float time_start;
    public State(int state_, float time_start_) {
        state = state_;
        time_start = time_start_;
    }
}

public class StateManager
{
    public List<State> states;
    List<int> statesIdToRemove, statesIdToAdd;
    public StateManager(Dictionary<string, int> STATES)
    {
         states = new List<State>();
         statesIdToRemove = new List<int>();
         statesIdToAdd = new List<int>();
    }

    public float GetTime(int stateId)
    {
        float time_start = states.Find(p => p.state == stateId).time_start;
        return (Time.time - time_start);
    }

    public float GetStartTime(int stateId)
    {
        return states.Find(p => p.state == stateId).time_start;
    }

    public void Set(int stateId) {
        states.Clear();
        states.Add(new State(stateId, Time.time));
    }

    public void Add(int stateId)
    {
        statesIdToAdd.Add(stateId);
    }

    public void Remove(int stateId)
    {
        statesIdToRemove.Add(stateId);
    }

    public bool Exists(int stateId) {
        return states.Exists(s => s.state == stateId);
    }

    public void Update() {
        foreach (int stateIdToRemove in statesIdToRemove) {
            State stateToRemove = states.Find(p => p.state == stateIdToRemove);
            if (stateToRemove != null)
                states.Remove(stateToRemove);
        }
        statesIdToRemove = new List<int>();
        
        foreach (int stateIdToAdd in statesIdToAdd) {
            states.Add(new State(stateIdToAdd, Time.time));
        }
        statesIdToAdd = new List<int>();
    }

    public IEnumerator GetEnumerator() {
        return states.GetEnumerator();
    }
}