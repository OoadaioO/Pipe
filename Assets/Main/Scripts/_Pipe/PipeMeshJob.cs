using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;


namespace xb.pipe.job {


    public struct PipeMeshJob : IJobFor {

        MeshStream streams;
        PipeQuadGenerator generator;

        public void Execute(int index) {
            generator.Execute(index, streams);
        }

        public static JobHandle SchedualParallel(PipeConfig config,Mesh mesh, Mesh.MeshData meshData, JobHandle dependency) {
            var job = new PipeMeshJob();

            job.generator.ringDistance = config.ringDistance;
            job.generator.curveRadius = config.curveRadius;
            job.generator.pipeRadius = config.pipeRadius;
            job.generator.curveSegmentCount = config.curveSegmentCount;
            job.generator.pipeSegmentCount = config.pipeSegmentCount;

            mesh.bounds = job.generator.Bounds;
            job.streams.Setup(meshData,
                    job.generator.Bounds,
                    job.generator.VertexCount,
                    job.generator.IndexCount
                );
            return job.ScheduleParallel(job.generator.QuadCount, 1, dependency);
        }


    }


    public struct PipeConfig {
        public float curveRadius, pipeRadius, ringDistance;

        public int curveSegmentCount, pipeSegmentCount;
    }


    public struct PipeQuadGenerator {

        public float ringDistance;
        public float curveRadius, pipeRadius;
        public int curveSegmentCount, pipeSegmentCount;


        public int VertexCount {
            get => QuadCount * 4;
        }
        public int IndexCount {
            get => QuadCount * 6;
        }

        public int QuadCount {
            get => curveSegmentCount * pipeSegmentCount;
        }

        public float Radius {
            get => curveRadius + pipeRadius;
        }

        public Bounds Bounds {
            get => new Bounds(Vector3.zero, new Vector3(Radius * 2f, Radius * 2f, Radius * 2f));
        }

        public void Execute(int index, MeshStream streams) {

            int vi = index * 4;
            int ti = index * 2;
            float uStep = ringDistance / curveRadius;
            float vStep = 2f * Mathf.PI / pipeSegmentCount;

            int v = index % pipeSegmentCount;
            int u = index / pipeSegmentCount;

            var vertex = new Vertex();

            var p0 = GetPointOnTorus(u * uStep, v * vStep);
            var p1 = GetPointOnTorus((u + 1) * uStep, v * vStep);
            var p2 = GetPointOnTorus(u * uStep, (v + 1) * vStep);
            var p3 = GetPointOnTorus((u + 1) * uStep, (v + 1) * vStep);

            var ab = p1 - p0;
            var ac = p2 - p0;


            vertex.position = p0;
            vertex.texCoord0 = float2(0f, 0f);
            vertex.normal = normalize(cross(ac, ab));
            //vertex.tangent.xyz = ab;
            streams.SetVertex(vi + 0, vertex);


            vertex.position = p1;
            vertex.texCoord0 = float2(0f, 1f);
            streams.SetVertex(vi + 1, vertex);


            vertex.position = p2;
            vertex.texCoord0 = float2(1f, 0f);
            streams.SetVertex(vi + 2, vertex);

            vertex.position = p3;
            vertex.texCoord0 = float2(1f, 1f);
            streams.SetVertex(vi + 3, vertex);

            // streams.SetTriangle(ti + 0, vi + int3(0, 2, 1)); // 反响 0,1,2
            // streams.SetTriangle(ti + 1, vi + int3(1, 2, 3)); // 反向 1,3,2


            streams.SetTriangle(ti + 0, vi + int3(0, 1, 2)); // 反响 0,1,2
            streams.SetTriangle(ti + 1, vi + int3(1, 3, 2)); // 反向 1,3,2


        }

        private Vector3 GetPointOnTorus(float u, float v) {
            Vector3 p;
            float r = curveRadius + pipeRadius * Mathf.Cos(v);
            p.x = r * Mathf.Sin(u);
            p.y = r * Mathf.Cos(u);
            p.z = pipeRadius * Mathf.Sin(v);

            return p;
        }

    }


    public struct MeshStream {

        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float3> vertexStream, normalStream;
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float4> tangentStream;
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float2> texCoord0Stream;
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<int3> triangles;

        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount) {

            NativeArray<VertexAttributeDescriptor> attributes = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            try {
                attributes[0] = new VertexAttributeDescriptor(
                        VertexAttribute.Position,
                        VertexAttributeFormat.Float32,
                        dimension: 3,
                        stream: 0
                    );
                attributes[1] = new VertexAttributeDescriptor(
                    VertexAttribute.Normal,
                    VertexAttributeFormat.Float32,
                    dimension: 3,
                    stream: 1
                );
                attributes[2] = new VertexAttributeDescriptor(
                    VertexAttribute.Tangent,
                    VertexAttributeFormat.Float32,
                    dimension: 4,
                    stream: 2
                );
                attributes[3] = new VertexAttributeDescriptor(
                    VertexAttribute.TexCoord0,
                    VertexAttributeFormat.Float32,
                    dimension: 2,
                    stream: 3
                );

                meshData.SetVertexBufferParams(vertexCount, attributes);
            } finally {

                attributes.Dispose();
            }

            vertexStream = meshData.GetVertexData<float3>(0);
            normalStream = meshData.GetVertexData<float3>(1);
            tangentStream = meshData.GetVertexData<float4>(2);
            texCoord0Stream = meshData.GetVertexData<float2>(3);

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);

            triangles = meshData.GetIndexData<int>().Reinterpret<int3>(4);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(
                0, 
                new SubMeshDescriptor(0,indexCount){
                    bounds = bounds,
                    vertexCount = vertexCount
                },
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices
            );

        }

        public void SetVertex(int index, Vertex vertex) {
            vertexStream[index] = vertex.position;
            normalStream[index] = vertex.normal;
            tangentStream[index] = vertex.tangent;
            texCoord0Stream[index] = vertex.texCoord0;

        }

        public void SetTriangle(int index, int3 triangle) {
            triangles[index] = triangle;
        }



    }

    public struct Vertex {
        public float3 position, normal;
        public float4 tangent;
        public float2 texCoord0;
    }


}