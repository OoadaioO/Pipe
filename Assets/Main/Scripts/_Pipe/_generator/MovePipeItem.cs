using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xb.pipe;
using xb.pipe.generator;

public class MovePipeItem : MonoBehaviour, IPlaceable {

    [SerializeField]
    private Transform rotater;

    [SerializeField]
    private float speed;


    private Pipe pipe;

    private float timer;

    private Vector3 startPosition;
    private float ringRotation;


    public void Position(Pipe pipe, float curveRotation, float ringRotation) {
        this.pipe = pipe;
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localPosition = new Vector3(0f, pipe.CurveRadius);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);

        startPosition = rotater.localPosition;
        this.ringRotation = ringRotation;
    }


    private void Update() {
        if (pipe == null) return;
        timer += Time.deltaTime;

        Vector3 moveDir = Mathf.Cos(speed * timer) * pipe.pipeRadius * new Vector3(0, Mathf.Cos(ringRotation), Mathf.Sin(ringRotation));
        rotater.localPosition = startPosition + moveDir;

    }



}
