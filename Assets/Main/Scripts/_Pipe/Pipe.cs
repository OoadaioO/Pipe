using UnityEngine;
using xb.pipe.generator;
using xb.pipe.job;


namespace xb.pipe {


    public class Pipe : MonoBehaviour {


        public float CurveRadius {
            get => curveRadius;
        }

        public float CurveAngle {
            get => curveAngle;
        }

        public float CurveSegmentCount {
            get => curveSegmentCount;
        }

        public float RelativeRotation {
            get => relativeRotation;
        }

        public float PipeRadius {
            get => pipeRadius;
        }

        public float pipeRadius; // 管道截面圆半径
        public int pipeSegmentCount;

        public float ringDistance;

        [SerializeField]
        private PipeItemGenerator[] generators;


        [SerializeField]
        private float minCurveRadius, maxCurveRadius;

        [SerializeField]
        private int minCurveSegmentCount, maxCurveSegmentCount;

        [SerializeField]
        private bool enableItems = true;

        private float curveRadius; // 管道圆半径
        private int curveSegmentCount;

        private float curveAngle; // 管道弧角度（角度值）

        private float relativeRotation;

        private Mesh mesh;


        private void Awake() {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Pipe";
        }

        public void Generate(bool withItems = true) {

            PipeItem[] items = GetComponentsInChildren<PipeItem>();
            for (int i = 0; i < items.Length; i++) {
                Destroy(items[i].gameObject);
            }

            curveRadius = Random.Range(minCurveRadius, maxCurveRadius);

            curveSegmentCount =
                Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);

            float uStep = ringDistance / curveRadius;
            curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));

            GenerateMesh();

            if (enableItems && withItems && generators.Length > 0) {
                generators[Random.Range(0, generators.Length)].GenerateItems(this);
            }


        }


        private void GenerateMesh() {
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);

            Mesh.MeshData meshData = meshDataArray[0];
            PipeMeshJob.SchedualParallel(
                    new PipeConfig() {
                        curveRadius = curveRadius,
                        pipeRadius = pipeRadius,
                        ringDistance = ringDistance,
                        curveSegmentCount = curveSegmentCount,
                        pipeSegmentCount = pipeSegmentCount
                    },
                    mesh,
                    meshData,
                    default
                ).Complete();

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);



        }


        // private void Update() {
        //     GenerateMesh();
        //     enabled = false;
        // }

        public void AlignWith(Pipe pipe) {

            // 增加管道多样性，进行的一定角度旋转
            relativeRotation =
                Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;

            transform.SetParent(pipe.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.curveAngle);
            transform.Translate(0f, pipe.curveRadius, 0f);
            transform.Rotate(relativeRotation, 0f, 0f);
            transform.Translate(0f, -curveRadius, 0f);
            transform.SetParent(pipe.transform.parent);
            transform.localScale = Vector3.one;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            // enabled = true;
        }
#endif

    }

}
