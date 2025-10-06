using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionReference interact;
    [SerializeField] private Cutter cutter;
    [SerializeField] private Cuttable cuttable;
    
    private PlayerInput m_playerInput;

    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        
        //m_playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (obj.action == interact.action)
        {
            CuttingManager.CutMesh(cuttable, cutter);
        }
    }

    private void Update()
    {
        CuttingManager.CutMesh(cuttable, cutter);
    }
}
