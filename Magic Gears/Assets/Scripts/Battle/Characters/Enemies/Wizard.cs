using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{

    void Start()
    {
        returnManaTurn = 999;  //Set to a high number since it will be returned back to 1 when the defensive attack is acalled again.
    }

    public int nightmareTurns;
    public int nightmareDamage;
    public int maxNightmareTurns;

    private int manaConsumed;
    private int returnManaTurn;

    public override void chooseAttack()
    {
        base.StateMachine3();

        if (currentAtk == CurrentAtk.BASIC)
        {
            Atk1();
        }
        else if (currentAtk == CurrentAtk.DEFENSE)
        {
            Atk2();
        }
        else if (currentAtk == CurrentAtk.OFFENSE)
        {
            Atk3();
        }
    }

    public override void Atk1()
    {
        if (battlesystem.state != BattleState.ENEMYTURN)
        {
            Debug.Log("Error");
            return;
        }
        StartCoroutine(EnemyAttack1());
    }
    public override void Atk2()
    {
        if (battlesystem.state != BattleState.ENEMYTURN)
        {
            Debug.Log("Error");
            return;
        }

        StartCoroutine(EnemyAttack2());
    }
    public override void Atk3()
    {
        if (battlesystem.state != BattleState.ENEMYTURN)
        {
            Debug.Log("Error");
            return;
        }
        StartCoroutine(EnemyAttack3());
    }

    public IEnumerator EnemyAttack1()
    {
        Debug.Log("Enemy unit attacks!");
        yield return new WaitForSeconds(1f);
        enemyAnimator.EnemyBasicAttack();
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Enemy gains " + manaCostBasic + "mana");
        UpdateEnemyMana(manaCostBasic);
        HUD.SetEnemyMana();
        playerAnimator.Damaged();
        bool isDead = currentPlayerUnit.TakeDamage(damageBasic);
        //HUD.SetPlayerHealth();
        if (isDead)
        {
            battlesystem.state = BattleState.LOST;
            Debug.Log("You lose!");
            battlesystem.EndBattle();
        }
        else
        {
            battlesystem.state = BattleState.PLAYERTURN;
            battlesystem.PlayerTurn();
        }
        checkManaToReturn();
        nigthmareIsOn();
    }

    public IEnumerator EnemyAttack2()
    {
        //Debug.Log("The mushroom is healing for " + maxHealTurns + " turns");
        yield return new WaitForSeconds(1f);
        enemyAnimator.EnemyBasicAttack();
        yield return new WaitForSeconds(0.5f);
        // Debug.Log("Enemy loose " + manaCostDefense + "mana");
        UpdateEnemyMana(manaCostDefense);
        HUD.SetEnemyMana();

        Debug.Log("BEFORE DEFENSE: " + Unit.currentPlayerMana + " health");
        //Consume half of the mana of the player. (Made as a bool because that's what the function returns)
        bool consumeHP = TakeDamage(Unit.currentPlayerMana * -1 / 2);
        Debug.Log("AFTER DEFENSE: " + Unit.currentPlayerMana + " health");



        //Store amount of mana to be returned next turn;
        manaConsumed = Unit.currentPlayerMana / 2;

        //Every turn will decrease the turn.
        returnManaTurn = 1;

        //Take all the mana of the player for that turn. Return half of it in the next turn
        currentPlayerUnit.UpdatePlayerMana(Unit.currentPlayerMana);

        //Update the HUD from taking the mana
        HUD.SetPlayerMana();
        HUD.SetEnemyHealth();

        battlesystem.state = BattleState.PLAYERTURN;
        battlesystem.PlayerTurn();
        checkManaToReturn();
        nigthmareIsOn();
    }

    public IEnumerator EnemyAttack3()
    {
        yield return new WaitForSeconds(1f);
        enemyAnimator.EnemyBasicAttack();
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Enemy loose " + manaCostOffense + "mana");
        UpdateEnemyMana(manaCostOffense);
        HUD.SetEnemyMana();
        playerAnimator.Damaged();

        if (nightmareTurns > 0)
        {
            //Wizard called the offense attack while player was still nightmare mode, increase by one turn the nigthmares
            nightmareTurns++;
        }
        else
        {
            nightmareTurns = maxNightmareTurns;
        }

        Debug.Log("The wizard has nightmare you for " + nightmareTurns + " turns");

        //NightmareIsOn is the function that will decrease the turns left and damage the player.
        nigthmareIsOn();
        checkManaToReturn();

    }

    //Player is still Nightmared. Take damage
    public void nigthmareIsOn()
    {
        if (nightmareTurns > 0)
        {

            Debug.Log("BEFORE Nightmare: " + currentPlayerUnit.currentHP + " health");
            bool isDead = currentPlayerUnit.TakeDamage(nightmareDamage);
            Debug.Log("AFTER Nightmare: " + currentPlayerUnit.currentHP + " health");
            if (isDead)
            {
                battlesystem.state = BattleState.LOST;
                Debug.Log("You lose!");
                battlesystem.EndBattle();
            }
            nightmareTurns--;
            playerAnimator.Damaged();
            battlesystem.state = BattleState.ENEMYTURN;
            enemyUnit.chooseAttack();
        }
    }

    public void checkManaToReturn()
    {
        if(returnManaTurn == 0)
        {
            UpdatePlayerMana(manaConsumed*-1);
            HUD.SetPlayerMana();
            returnManaTurn = 999; //Set to a high number since it will be returned back to 1 when the defensive attack is acalled again.
        }
        returnManaTurn--;
    }
}