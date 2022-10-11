using UnityEngine;
using UnityEngine.InputSystem;



	public class PlayerInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 cursor_location;
		public bool move;

		[Header("Chat Input Values")]
		public bool send;




    public void OnMove(InputValue value)
		{
			MoveInput(value.isPressed);
		}


    public void OnCursor(InputValue value)
		{
			CursorInput(value.Get<Vector2>());
		}
    public void OnSend(InputValue value)
    {
        SendInput(value.isPressed);
    }
    public void MoveInput(bool _move)
    {
        move = _move;
    }
    public void CursorInput(Vector2 cursor)
		{
			cursor_location = cursor;
		}
    public void SendInput(bool _send)
    {
        send = _send;
    }

}
	
