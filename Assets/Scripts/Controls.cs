using UnityEngine;

namespace Avenyrh
{
	public abstract class Controls
	{
		public abstract bool Left();
		public abstract bool Right();
		public abstract bool Down();
		public abstract bool Up();
		public abstract bool RotateClockwise();
		public abstract bool RotateTrigo();
        public abstract bool Store();

        protected const float Deadzone = 0.25f;
	}

    public class Controls_WASD : Controls
    {
        public override bool Left() => Input.GetKey(KeyCode.A);
        public override bool Right() => Input.GetKey(KeyCode.D);
        public override bool Down() => Input.GetKey(KeyCode.S);
        public override bool Up() => Input.GetKeyDown(KeyCode.W);
        public override bool RotateClockwise() => Input.GetKeyDown(KeyCode.E);
        public override bool RotateTrigo() => Input.GetKeyDown(KeyCode.Q);
        public override bool Store() => Input.GetKeyDown(KeyCode.R);
    }

    public class Controls_IJKL : Controls
    {
        public override bool Left() => Input.GetKey(KeyCode.J);
        public override bool Right() => Input.GetKey(KeyCode.L);
        public override bool Down() => Input.GetKey(KeyCode.K);
        public override bool Up() => Input.GetKeyDown(KeyCode.I);
        public override bool RotateClockwise() => Input.GetKeyDown(KeyCode.U);
        public override bool RotateTrigo() => Input.GetKeyDown(KeyCode.O);
        public override bool Store() => Input.GetKeyDown(KeyCode.P);
    }

    public class Controls_Controller1 : Controls
    {
        private bool _wasUp = false;

        public override bool Left() => Input.GetAxis("Horizontal_P1") < -Deadzone;
        public override bool Right() => Input.GetAxis("Horizontal_P1") > Deadzone;
        public override bool Down() => Input.GetAxis("Vertical_P1") < -Deadzone;
        public override bool Up()
        {
            bool isUp = Input.GetAxis("Vertical_P1") > Deadzone;
            bool wasUp = _wasUp;
            _wasUp = isUp;
            return !wasUp && isUp;
        }
        public override bool RotateClockwise() => Input.GetKeyDown(KeyCode.Joystick1Button5);
        public override bool RotateTrigo() => Input.GetKeyDown(KeyCode.Joystick1Button4);
        public override bool Store() => Input.GetKeyDown(KeyCode.Joystick1Button0);
    }

    public class Controls_Controller2 : Controls
    {
        private bool _wasUp = false;

        public override bool Left() => Input.GetAxis("Horizontal_P2") < -Deadzone;
        public override bool Right() => Input.GetAxis("Horizontal_P2") > Deadzone;
        public override bool Down() => Input.GetAxis("Vertical_P2") < -Deadzone;
        public override bool Up()
        {
            bool isUp = Input.GetAxis("Vertical_P2") > Deadzone;
            bool wasUp = _wasUp;
            _wasUp = isUp;
            return !wasUp && isUp;
        }
        public override bool RotateClockwise() => Input.GetKeyDown(KeyCode.Joystick2Button5);
        public override bool RotateTrigo() => Input.GetKeyDown(KeyCode.Joystick2Button4);
        public override bool Store() => Input.GetKeyDown(KeyCode.Joystick2Button0);
    }

    public enum EControl
	{
        NONE,
		WASD,
		IJKL,
		CONTROLLER_1,
		CONTROLLER_2
	}
}
