using UnityEngine;
using xb.pipe.generator;
using xb.pipe.job;


namespace xb.pipe {

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PipeMesh : MonoBehaviour {


        public float curveRadius, pipeRadius, ringDistance;

        public int curveSegmentCount, pipeSegmentCount;


        private Mesh mesh;


        private void Awake() {
            mesh = new Mesh() {
                name = "Pipe"
            };
            GetComponent<MeshFilter>().mesh = mesh;
        }

        private void Update() {
            GenerateMesh();
            enabled = false;
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


        private void OnValidate() {
            enabled = true;
        }
    }




}