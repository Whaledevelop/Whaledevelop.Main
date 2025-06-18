using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Whaledevelop
{
    public static class InputSystemUtility
    {
        public static float GetMouseWorldX(Camera camera, Vector3 playerPosition)
        {
            var ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            var plane = new Plane(Vector3.up, playerPosition);
            if (plane.Raycast(ray, out var distance))
            {
                var worldPoint = ray.GetPoint(distance);
                return worldPoint.x;
            }

            return playerPosition.x;
        }

        
        public static void BindAction(InputActionMap actionMap, string name,
            Action<InputAction.CallbackContext> performedCallback,
            Action<InputAction.CallbackContext> canceledCallback = null)
        {
            var action = actionMap.FindAction(name, true);

            action.performed += performedCallback;
            if (canceledCallback != null)
            {
                action.canceled += canceledCallback;
            }
        }

        
        public static void UnbindAction(InputActionMap actionMap, string name,
            Action<InputAction.CallbackContext> performedCallback,
            Action<InputAction.CallbackContext> canceledCallback = null)
        {
            var action = actionMap.FindAction(name, true);

            action.performed -= performedCallback;
            if (canceledCallback != null)
            {
                action.canceled -= canceledCallback;
            }
        }
    }
}