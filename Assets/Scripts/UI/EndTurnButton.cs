using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
   public void OnEndTurnButtonClick()
   {
       EnemyTurnGA enemyTurnGA = new();
       ActionSystem.Instance.Perform(enemyTurnGA);
   }
   
}
