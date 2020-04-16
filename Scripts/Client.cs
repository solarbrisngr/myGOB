using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    Goal[] mGoals;
    Action[] mActions;
    Action mChangeOverTime;
    const float TICK_LENGTH = 5.0f;

    public GameObject client;

    // Start is called before the first frame update
    void Start()
    {
        // my inital motives/goals
        mGoals = new Goal[3];
        mGoals[0] = new Goal("Eat", 4);
        mGoals[1] = new Goal("Sleep", 3);
        mGoals[2] = new Goal("Bathroom", 3);

        //// the actions I know how to do
        mActions = new Action[6];

        mActions[0] = new Action("eat a snack", PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Capsule));
        mActions[0].targetGoals.Add(new Goal("Eat", -2f));
        mActions[0].targetGoals.Add(new Goal("Sleep", -1f));
        mActions[0].targetGoals.Add(new Goal("Bathroom", +1f));

        mActions[1] = new Action("sleep in the bed", PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube));
        mActions[1].targetGoals.Add(new Goal("Eat", +2f));
        mActions[1].targetGoals.Add(new Goal("Sleep", -4f));
        mActions[1].targetGoals.Add(new Goal("Bathroom", +2f));

        mActions[2] = new Action("visit the bathroom", PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cylinder));
        mActions[2].targetGoals.Add(new Goal("Eat", 0f));
        mActions[2].targetGoals.Add(new Goal("Sleep", 0f));
        mActions[2].targetGoals.Add(new Goal("Bathroom", -4f));

        mActions[3] = new Action("eat dinner", PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Plane));
        mActions[3].targetGoals.Add(new Goal("Eat", -2f));
        mActions[3].targetGoals.Add(new Goal("Sleep", -1f));
        mActions[3].targetGoals.Add(new Goal("Bathroom", +1f));

        mActions[4] = new Action("Nap on the couch", PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Quad));
        mActions[4].targetGoals.Add(new Goal("Eat", +2f));
        mActions[4].targetGoals.Add(new Goal("Sleep", -4f));
        mActions[4].targetGoals.Add(new Goal("Bathroom", +2f));

        mActions[5] = new Action("Take A Shower", PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere));
        mActions[5].targetGoals.Add(new Goal("Eat", 0f));
        mActions[5].targetGoals.Add(new Goal("Sleep", 0f));
        mActions[5].targetGoals.Add(new Goal("Bathroom", -4f));

        // the rate my goals change just as a result of time passing
        mChangeOverTime = new Action("tick");
        mChangeOverTime.targetGoals.Add(new Goal("Eat", +4f));
        mChangeOverTime.targetGoals.Add(new Goal("Sleep", +1f));
        mChangeOverTime.targetGoals.Add(new Goal("Bathroom", +2f));

        Debug.Log("Starting clock. One hour will pass every " + TICK_LENGTH + " seconds.");
        InvokeRepeating("Tick", 0f, TICK_LENGTH);

        Debug.Log("Hit E to do something.");
    }

    void Tick()
    {
        // apply change over time
        foreach (Goal goal in mGoals)
        {
            goal.value += mChangeOverTime.GetGoalChange(goal);
            //Debug.Log(mChangeOverTime.GetGoalChange(goal));
            goal.value = Mathf.Max(goal.value, 0);
        }

        // print results
        PrintGoals();
    }

    void PrintGoals()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        Debug.Log(goalString);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("-- INITIAL GOALS --");
            //PrintGoals();

            Action theThingToDo = ChooseAction(mActions, mGoals);
            //Debug.Log("-- BEST ACTION --");
            Debug.Log("I think I will " + theThingToDo.name);
            client.GetComponent <MeshFilter>().mesh = theThingToDo.shape;

            // do the thing
            foreach (Goal goal in mGoals)
            {
                goal.value += theThingToDo.GetGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }

            // Debug.Log("-- NEW GOALS --");
            PrintGoals();
        }
    }

    Action ChooseAction(Action[] actions, Goal[] goals)
    {
        // find the action leading to the lowest discontentment
        Action bestAction = null;
        float bestValue = float.PositiveInfinity;

        foreach (Action action in actions)
        {
            float thisValue = Discontentment(action, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
            }
        }

        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        // keep a running total
        float discontentment = 0f;

        // loop through each goal
        foreach (Goal goal in goals)
        {
            // calculate the new value after the action
            float newValue = goal.value + action.GetGoalChange(goal);
            newValue = Mathf.Max(newValue, 0);

            // get the discontentment of this value
            discontentment += goal.GetDiscontentment(newValue);
        }

        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach (Goal goal in mGoals)
        {
            total += (goal.value * goal.value);
        }
        return total;
    }
}