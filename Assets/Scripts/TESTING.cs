using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TESTING : MonoBehaviour
{
    public GameController m_gameController;
    public Backend m_backend;
    public string m_createCharTest = "Player,1,5,5,1,0,Player_1,0|0|255";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            StartCoroutine(TestingSequence());
        }
    }

    public IEnumerator TestingSequence()
    {
        // GameController
        GameObject goPlayer = m_gameController.CreateCharacter(true, 0, m_createCharTest.Split(','));
        Assert.IsNotNull(goPlayer);
        Assert.IsNotNull(m_gameController.m_playersDict[goPlayer.GetComponent<PlayerControls>().m_id]);

        // Check AllData
        Assert.AreEqual(goPlayer.GetComponent<PlayerControls>().GetAllData(), "Update,0,5,5,1,0");

        GameObject goOther = m_gameController.CreateCharacter(false, 1, m_createCharTest.Split(','));
        Assert.IsNotNull(goOther);
        Assert.IsNotNull(m_gameController.m_playersDict[goOther.GetComponent<PlayerControls>().m_id]);

        yield return new WaitForSeconds(1);

        Assert.AreEqual(m_gameController.GetPlayerChangedData(), "Unchanged");

        // Move right
        float xPositionPlayer = goPlayer.transform.position.x;
        float xPositionOther = goOther.transform.position.x;
        float keepLoopingTime = 1f;
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = 'D';
        while (keepLoopingTime > 0)
        {
            keepLoopingTime -= Time.deltaTime;
            yield return null;
        }
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = ' ';
        Assert.AreNotEqual(m_gameController.GetPlayerChangedData(), "Unchanged");
        Assert.AreEqual(goOther.transform.position.x, xPositionOther);
        Assert.IsTrue(goPlayer.transform.position.x > xPositionPlayer);


        // Check if sprite flipped - get value
        float playerSpriteTransformX = goPlayer.GetComponent<PlayerControls>().m_playerSprite.transform.localScale.x;

        // Move left
        xPositionPlayer = goPlayer.transform.position.x; 
        keepLoopingTime = 1f;
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = 'A';
        while (keepLoopingTime > 0)
        {
            keepLoopingTime -= Time.deltaTime;
            yield return null;
        }
        Assert.IsTrue(goPlayer.GetComponent<PlayerControls>().m_isMoving);
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = ' ';
        Assert.AreNotEqual(m_gameController.GetPlayerChangedData(), "Unchanged");

        Assert.AreEqual(goOther.transform.position.x, xPositionOther);

        // Check if sprite flipped - check value
        Assert.AreEqual(-playerSpriteTransformX, goPlayer.GetComponent<PlayerControls>().m_playerSprite.transform.localScale.x);

        yield return null;
        Assert.IsFalse(goPlayer.GetComponent<PlayerControls>().m_isMoving);
        // Move up
        float yPositionPlayer = goPlayer.transform.position.y; 
        float yPositionOther = goOther.transform.position.y; 
        keepLoopingTime = 1f;
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = 'W';
        while (keepLoopingTime > 0)
        {
            keepLoopingTime -= Time.deltaTime;
            yield return null;
        }
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = ' ';
        Assert.AreNotEqual(m_gameController.GetPlayerChangedData(), "Unchanged");
        Assert.IsTrue(goPlayer.transform.position.y > yPositionPlayer);

        // Move down
        yPositionPlayer = goPlayer.transform.position.y;
        keepLoopingTime = 1f;
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = 'S';
        while (keepLoopingTime > 0)
        {
            keepLoopingTime -= Time.deltaTime;
            yield return null;
        }
        goPlayer.GetComponent<PlayerControls>().m_testingMovement = ' ';
        Assert.AreNotEqual(m_gameController.GetPlayerChangedData(), "Unchanged");
        Assert.IsTrue(goPlayer.transform.position.y < yPositionPlayer);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(goPlayer.GetComponent<PlayerControls>().GetAllData(), $"Update,0,{goPlayer.transform.position.x},{goPlayer.transform.position.y},-1,0");


        yield return new WaitForSeconds(5);
        m_gameController.DestroyPlayer();
        yield return new WaitForSeconds(1);
        Assert.AreNotEqual(goPlayer.GetType(), typeof(PlayerControls));
    }
}
