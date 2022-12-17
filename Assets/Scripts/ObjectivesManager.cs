using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    public struct Objective
    {
        public String   name;
        public bool     reached;
    }

    protected List<Objective> objectives;
    protected Objective curObjective;
    protected bool initDone = false;

    // Start is called before the first frame update
    void Start()
    {
        objectives = new List<Objective>();

        // On récupère les objectifs = les astres à trouver
        foreach (GameObject body in GameObject.FindGameObjectsWithTag("Bodies"))
        {
            Objective obj = new Objective();

            obj.name = body.name;
            obj.reached = false;

            objectives.Add(obj);
        }

        // On réarrange la liste par ordre alphabétique
        objectives = objectives.OrderBy(x => x.name).ToList();

        curObjective = objectives.First();

        initDone = true;
    }

    public Objective GetNextObjective()
    {
        for(int i = 0; i < objectives.Count; i++)
        {
            if (!objectives[i].reached)
            {
                curObjective = objectives[i];
                return curObjective;
            }
        }
        return new Objective();
    }

    public Objective GetCurObjective()
    {
        return curObjective;
    }

    public bool initOK()
    {
        return initDone;
    }

    public void SetObjectiveReached(Objective obj)
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].name.Equals(obj.name))
            {
                obj.reached = true;
                objectives[i] = obj;
            }
        }
    }
}
