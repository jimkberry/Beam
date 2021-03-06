using UnityEngine;
using System.Collections;
using BeamGameCode;

public class ConnectBtn : UIBtn  {

	protected BeamMain _main = null;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_main = BeamMain.GetInstance();

	}

	protected void _DoConnect()
	{
		_main.beamApp.OnSwitchModeReq(BeamModeFactory.kConnect, null);
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Return))
			_DoConnect();

	}

	public override void doSelect()
	{
		_DoConnect();
	}
}

