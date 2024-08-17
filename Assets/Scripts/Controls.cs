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
	}

    public class Controls_WASD : Controls
    {
        public override bool Left() => Input.GetKeyDown(KeyCode.A);
        public override bool Right() => Input.GetKeyDown(KeyCode.D);
        public override bool Down() => Input.GetKey(KeyCode.S);
        public override bool Up() => Input.GetKeyDown(KeyCode.W);
        public override bool RotateClockwise() => Input.GetKeyDown(KeyCode.E);
        public override bool RotateTrigo() => Input.GetKeyDown(KeyCode.Q);
        public override bool Store() => Input.GetKeyDown(KeyCode.R);
    }

    public class Controls_IJKL : Controls
    {
        public override bool Left() => Input.GetKeyDown(KeyCode.J);
        public override bool Right() => Input.GetKeyDown(KeyCode.L);
        public override bool Down() => Input.GetKey(KeyCode.K);
        public override bool Up() => Input.GetKeyDown(KeyCode.I);
        public override bool RotateClockwise() => Input.GetKeyDown(KeyCode.U);
        public override bool RotateTrigo() => Input.GetKeyDown(KeyCode.O);
        public override bool Store() => Input.GetKeyDown(KeyCode.P);
    }

    public enum EControl
	{
		WASD,
		IJKL,
		ARROWS,
		CONTROLLER_1,
		CONTROLLER_2
	}
}
