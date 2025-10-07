using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference interact;
    [SerializeField] private Cutter cutter;
    
    private PlayerInput m_playerInput;

    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        
        m_playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (obj.started && obj.action == interact.action)
        {
            foreach (Cuttable cuttable in FindObjectsByType<Cuttable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                CuttingManager.CutMesh(cuttable, cutter);
            }
        }
    }

    private void Update()
    {
        //CuttingManager.CutMesh(cuttable, cutter);
    }
}
