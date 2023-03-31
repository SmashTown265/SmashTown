using UnityEditor;
using UnityEngine;

public class CameraMenu2D : MonoBehaviour
{
    
    [MenuItem("Tools/2D Camera/Player/Setup")]
    static void SetupPlayer() {
        
        // Attempt to setup the player object
        // Find the player first
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null) {
            // If we don't find the player by tag, try to find it by name
            playerGo = GameObject.Find("Player");
            
            // If we don't find it with this mechanism, throw an error
            if (playerGo == null) {
                Debug.LogError("Failed to find the player! Either add the script 'Player2D' to your player or add the Player Tag to your players GameObject and run this script again!");
                return;
            }
        }

      
    }

    [MenuItem("Tools/2D Camera/Camera/Setup")]
    static void SetepCamera()
    {
        GameObject cameraGo = new GameObject("2D Camera");
        cameraGo.transform.position = new Vector3(0, 0, -10);
        cameraGo.tag = "MainCamera";

        Camera camera = cameraGo.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5;
        camera.depth = -1;
        
        cameraGo.AddComponent<AudioListener>();
        cameraGo.AddComponent<Camera2D>();

        Selection.activeGameObject = cameraGo;
    }
}
