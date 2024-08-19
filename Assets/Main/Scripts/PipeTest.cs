using UnityEngine;
using xb.pipe;

public class PipeTest : MonoBehaviour {


    [SerializeField]
    private Pipe pipeA;

    [SerializeField]
    private Pipe pipeB;

    [SerializeField]
    private MovePipeItem pipeItem;

    [SerializeField]
    private float ringRotation;

    private void Awake() {

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            pipeA.Generate(false);
            pipeB.Generate(false);
            pipeB.AlignWith(pipeA);
        } else if (Input.GetKeyDown(KeyCode.M)) {
            pipeItem.Position(pipeB, pipeB.CurveAngle, ringRotation);
        }
    }



}
