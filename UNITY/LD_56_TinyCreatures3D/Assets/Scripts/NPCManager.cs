using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    private static List<NPCMouseController> playerTeamNPCs = new List<NPCMouseController>();
    private static List<NPCEnemyController> enemyTeamNPCs = new List<NPCEnemyController>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void RegisterNPC(NPCMouseController mouse)
    {
        playerTeamNPCs.Add(mouse);
    }

    public static void RegisterNPC(NPCEnemyController enemy)
    {
        enemyTeamNPCs.Add(enemy);
    }
}