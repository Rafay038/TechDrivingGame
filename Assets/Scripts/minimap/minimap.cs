using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject minimap; // Reference to the minimap GameObject, can be used to enable/disable the minimap

    public Transform player;              // Reference to the player transform to track their position and rotation
    public RectTransform minimapRect;    // UI RectTransform representing the minimap area on the screen
    public float mapScale = 1f;          // Scale factor to convert world coordinates to minimap coordinates
    public bool rotateWithPlayer = true; // If true, the minimap rotates to match the player's rotation

    private List<MinimapIcon> icons = new List<MinimapIcon>(); // List of all icons currently displayed on the minimap

    void Update()
    {
        // Update each minimap icon's position and rotation based on the player's position and rotation
        foreach (var icon in icons)
        {
            // Calculate offset of the icon's target relative to the player
            Vector3 offset = icon.target.position - player.position;

            if (rotateWithPlayer)
            {
                // Rotate the offset vector inversely by the player's Y rotation so icons rotate with the minimap
                offset = Quaternion.Euler(0, -player.eulerAngles.y, 0) * offset;
            }

            // Convert the 3D offset to 2D minimap position (X and Z axes) and apply scaling
            Vector2 minimapPos = new Vector2(offset.x, offset.z) * mapScale;

            // Set the anchored position of the UI icon on the minimap
            icon.uiIcon.anchoredPosition = minimapPos;

            // Set the icon rotation: if minimap rotates with player, keep icons upright; otherwise, match target rotation
            icon.uiIcon.rotation = rotateWithPlayer ? Quaternion.identity : icon.target.rotation;
        }
    }

    // Register a new icon to be tracked and displayed on the minimap
    public void RegisterIcon(MinimapIcon icon)
    {
        icons.Add(icon);
    }

    // Remove an icon from the minimap tracking list
    public void UnregisterIcon(MinimapIcon icon)
    {
        icons.Remove(icon);
    }
}

public class MinimapIcon : MonoBehaviour
{
    public Transform target;             // The world object this icon represents on the minimap
    public RectTransform uiIcon;         // The UI element representing the icon on the minimap

    void Start()
    {
        // Register this icon with the MinimapManager when it is initialized
        FindObjectOfType<MinimapManager>().RegisterIcon(this);
    }

    void OnDestroy()
    {
        // Unregister this icon from the MinimapManager when it is destroyed to avoid errors
        FindObjectOfType<MinimapManager>().UnregisterIcon(this);
    }
}

public class Minimap : MonoBehaviour {

    public Transform player;              // Reference to the player transform to follow their position
    [SerializeField]
    private GameObject minimapView;      // The GameObject representing the minimap view, can be enabled/disabled

    void LateUpdate()
    {
        // Update the minimap's world position to follow the player horizontally
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Keep the minimap at a fixed height (e.g., above the map)
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(90, player.eulerAngles.y, 0); // Rotate the minimap to face the same direction as the player
    }
}