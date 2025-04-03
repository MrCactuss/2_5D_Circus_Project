using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDetectorScript : MonoBehaviour
{
    DiceRollScript diceRollScript;

    void Awake()
    {
        diceRollScript = FindObjectOfType<DiceRollScript>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (diceRollScript != null)
            if (diceRollScript.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                diceRollScript.isLanded = true;
                diceRollScript.diceFaceNum = other.name;

            }
            else
                diceRollScript.isLanded = false;

        else
            Debug.LogError("DiceRollScript not found in a scene!");

    }
}
