using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Project.Scripts.Core.Services.AssetsProvider
{
    public class TextureAssetProvider : IAssetsProvider<Texture, string>
    {
        public async UniTask<Texture> GetTexture(string assetContainer, CancellationToken token)
        {
            Texture loadedTexture = null;
            
            try
            {
                var request = UnityWebRequestTexture.GetTexture(assetContainer);

                await request.SendWebRequest().WithCancellation(token);

                if (request.result == UnityWebRequest.Result.Success && !token.IsCancellationRequested)
                {
                    var texture = DownloadHandlerTexture.GetContent(request);
                    loadedTexture = texture;
                }
            }
            catch (OperationCanceledException oce)
            {
                // it's okay
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            return loadedTexture;
        }
    }
}