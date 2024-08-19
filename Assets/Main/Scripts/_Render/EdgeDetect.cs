using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetect : ScriptableRendererFeature {


  

    class EdgeDetectRenderPass : ScriptableRenderPass {
        private RenderTargetIdentifier source;
        private RenderTargetHandle destination;

        public EdgeDetectSetting settings;
        protected Material outlineMaterial;

        RenderTargetHandle temporaryColorTexture;


        public EdgeDetectRenderPass(Material outlineMaterial) {
            this.outlineMaterial = outlineMaterial;
        }

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination) {
            this.source = source;
            this.destination = destination;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        // 这个方法会在执行渲染通道前调用
        // 可以用于配制渲染目标和清理状态。并且也可以创建临时渲染纹理
        // 当为方法为空的时候，表示通道将当前相机渲染对象作为渲染的目标
        // 不要调用 CommandBuff.SetRenderTarget，使用ConfigureTarget和ConfigClear设置渲染目标
        // 渲染管线会确保目标正确设置和清理
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

            CommandBuffer cmd = CommandBufferPool.Get("_EdgeDetectRenderPass");

            RenderTextureDescriptor opaqueDescriptor =  renderingData.cameraData.cameraTargetDescriptor;

            if(destination == RenderTargetHandle.CameraTarget){
                // 渲染目标如果是相机，通过临时渲染纹理渲染
                // 通过相机目标纹理描述，创建临时渲染纹理
                cmd.GetTemporaryRT(temporaryColorTexture.id,opaqueDescriptor,FilterMode.Point);
                // 使用material处理临时渲染纹理
                Blit(cmd,source,temporaryColorTexture.Identifier(),outlineMaterial,0);
                // 拷贝到目标纹理
                Blit(cmd,temporaryColorTexture.Identifier(),source);
            }else{
                Blit(cmd,source,destination.Identifier(),outlineMaterial,0);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);


        }

        public override void FrameCleanup(CommandBuffer cmd) {
            if(destination == RenderTargetHandle.CameraTarget){
                cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
            }
        }
    }

  [System.Serializable]
    public class EdgeDetectSetting {
        public Material material = null;
    }


    public EdgeDetectSetting settings = new EdgeDetectSetting();
    
    EdgeDetectRenderPass outlinePass;

    RenderTargetHandle outlineTexture;


    public override void Create() {

        outlinePass = new EdgeDetectRenderPass(settings.material);
        outlinePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        outlinePass.settings = settings;


    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {

        if (settings.material == null) return;

        if (renderingData.cameraData.isPreviewCamera) return;

        outlinePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        renderer.EnqueuePass(outlinePass);

        outlineTexture.Init("_MainTex");
    }

}
