using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    PlayerInput playerInput;

    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogWarning("Warning: Multiple input managers found in scene. Try avioding having" +
                "multiple input managers in the project at once");
            Destroy(gameObject);
        }
    }
}
