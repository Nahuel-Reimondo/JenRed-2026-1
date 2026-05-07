using UnityEngine;

public class PlayerInput : IMovementInput
{
   public float Horizontal => Input.GetAxisRaw("Horizontal");

   public float Vertical => Input.GetAxisRaw("Vertical");
}
