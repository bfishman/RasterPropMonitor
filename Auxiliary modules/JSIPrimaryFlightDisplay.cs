using UnityEngine;

namespace JSI
{
	public class JSIPrimaryFlightDisplay: InternalModule
	{
		[KSPField]
		public string horizonTexture;

		private Material horizonMaterial;
		private int angle;

		private RasterPropMonitorComputer comp;

		public bool RenderPFD(RenderTexture screen)
		{
			if (screen == null)
				return false;
			GL.Clear(true, true, Color.blue);



			DrawHorizon(angle);
			return true;
		}
		// Boy, what a complicated way to do it.
		private void DrawHorizon(float rollAngle, float pitchAngle)
		{

			Vector3 bottomLeft = Vector3.zero;
			Vector3 topLeft = new Vector3(0f, 1f, 0); 
			Vector3 topRight = new Vector3(1f, 1f, 0);
			Vector3 bottomRight = new Vector3(1f, 0f, 0);

			Vector3 center = new Vector3(0.5f, 0.5f, 0);
			Quaternion angleQuat = Quaternion.Euler(0, 0, rollAngle);


			bottomLeft = RotateAroundPoint(bottomLeft, center, angleQuat);
			topLeft = RotateAroundPoint(topLeft, center, angleQuat);
			bottomRight = RotateAroundPoint(bottomRight, center, angleQuat);
			topRight = RotateAroundPoint(topRight, center, angleQuat);

			GL.PushMatrix();
			GL.LoadOrtho();
			horizonMaterial.SetPass(0);

			GL.Begin(GL.QUADS);
			GL.Color(Color.white);
			// Examples seem to do it clockwise.
			GL.TexCoord2(0, 0);
			GL.Vertex(bottomLeft);
			GL.TexCoord2(0, 1);
			GL.Vertex(topLeft);
			GL.TexCoord2(1, 1);
			GL.Vertex(topRight);
			GL.TexCoord2(1, 0);
			GL.Vertex(bottomRight);
			GL.End();

			GL.PopMatrix();

		}

		private static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
		{
			return angle * (point - pivot) + pivot;
		}

		public override void OnUpdate()
		{
			angle++;
			if (angle > 359)
				angle = 0;
		}

		public void Start()
		{
			horizonMaterial = new Material(Shader.Find("KSP/Unlit"));
			horizonMaterial.SetTexture("_MainTex", GameDatabase.Instance.GetTexture(horizonTexture, false));
			comp = JUtil.GetComputer(internalProp);
		}
	}
}

