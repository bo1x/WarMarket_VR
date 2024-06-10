using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckMap : MonoBehaviour
{
    #region Variables
    // Variables to store the dimensions of the map
    public float longWidth = 10.0f; // Map width
    public float longHeight = 10.0f; // Map height

    // Scene names for the four directions
    public string rightSceneName = "right";
    public string leftSceneName = "left";
    public string upSceneName = "up";
    public string downSceneName = "down";
    public string sceneMap;

    // Flag to indicate if the player is near the edge of the map
    private bool nearEdge = false;
    #endregion

    #region Methods
    // Method to check if the player is near the edge of the map
    public bool IsAtEdge(Vector2 playerPosition)
    {
        float x = playerPosition.x;
        float y = playerPosition.y;

        // Minimum distance from the edge to consider the player near it
        float minimumDistance = 1.0f;

        // Check if the player is near any edge
        if (x <= minimumDistance)
        {
            if (rightSceneName != null)
            {
                nearEdge = true;
                sceneMap = leftSceneName;
            }
        }
        else if (x >= longWidth - minimumDistance)
        {
            if (leftSceneName != null)
            {
                nearEdge = true;
                sceneMap = rightSceneName;
            }
        }
        else if (y <= minimumDistance)
        {
            if (downSceneName != null)
            {
                nearEdge = true;
                sceneMap = downSceneName;
            }
        }
        else if (y >= longHeight - minimumDistance)
        {
            if (upSceneName != null)
            {
                nearEdge = true;
                sceneMap = upSceneName;
            }
        }
        else
        {
            nearEdge = false;
        }

        PromptMapChange(nearEdge);
        return nearEdge;
    }

    // Method to prompt the player to change the map
    public void PromptMapChange(bool nearEdge)
    {
        if (nearEdge)
        {
            Debug.Log("You are at the edge of the map. Do you want to change the map?");
            
            if (true) 
            {
                ChangeMap();
            }
        }
    }

    // Method to change the map
    public void ChangeMap()
    {
        SceneManager.LoadScene(sceneMap);
    }
    #endregion
}
