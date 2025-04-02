using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

public enum ResourceLoadMode
{
    Resources,
    Addressables
}

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ResourceManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ResourceManager");
                    _instance = go.AddComponent<ResourceManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [SerializeField] private ResourceLoadMode _loadMode = ResourceLoadMode.Addressables;
    private Dictionary<string, AsyncOperationHandle> _addressableHandles = new Dictionary<string, AsyncOperationHandle>();

    public ResourceLoadMode LoadMode
    {
        get => _loadMode;
        set => _loadMode = value;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            ReleaseAllAddressables();
            _instance = null;
        }
    }

    #region Generic Load Methods

    public void LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
    {
        switch (_loadMode)
        {
            case ResourceLoadMode.Resources:
                LoadFromResourcesAsync(path, onComplete);
                break;
            case ResourceLoadMode.Addressables:
                LoadFromAddressablesAsync(path, onComplete);
                break;
        }
    }

    public T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        switch (_loadMode)
        {
            case ResourceLoadMode.Resources:
                return LoadFromResources<T>(path);
            case ResourceLoadMode.Addressables:
                return LoadFromAddressables<T>(path);
            default:
                return null;
        }
    }

    public void InstantiateAsync(string path, Action<GameObject> onComplete, Transform parent = null)
    {
        switch (_loadMode)
        {
            case ResourceLoadMode.Resources:
                InstantiateFromResourcesAsync(path, onComplete, parent);
                break;
            case ResourceLoadMode.Addressables:
                InstantiateFromAddressablesAsync(path, onComplete, parent);
                break;
        }
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        switch (_loadMode)
        {
            case ResourceLoadMode.Resources:
                return InstantiateFromResources(path, parent);
            case ResourceLoadMode.Addressables:
                return InstantiateFromAddressables(path, parent);
            default:
                return null;
        }
    }

    #endregion

    #region Resources System Methods

    private void LoadFromResourcesAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(path);
        request.completed += (op) =>
        {
            T asset = (op as ResourceRequest).asset as T;
            onComplete?.Invoke(asset);
        };
    }

    private T LoadFromResources<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }

    private void InstantiateFromResourcesAsync(string path, Action<GameObject> onComplete, Transform parent = null)
    {
        LoadFromResourcesAsync<GameObject>(path, (prefab) =>
        {
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, parent);
                onComplete?.Invoke(instance);
            }
            else
            {
                Debug.LogError($"Failed to load prefab from Resources: {path}");
                onComplete?.Invoke(null);
            }
        });
    }

    private GameObject InstantiateFromResources(string path, Transform parent = null)
    {
        GameObject prefab = LoadFromResources<GameObject>(path);
        if (prefab != null)
        {
            return Instantiate(prefab, parent);
        }
        Debug.LogError($"Failed to load prefab from Resources: {path}");
        return null;
    }

    #endregion

    #region Addressables System Methods

    private void LoadFromAddressablesAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
    {
        if (_addressableHandles.TryGetValue(path, out AsyncOperationHandle existingHandle))
        {
            if (existingHandle.IsDone)
            {
                onComplete?.Invoke((T)existingHandle.Result);
            }
            else
            {
                existingHandle.Completed += (handle) =>
                {
                    onComplete?.Invoke((T)handle.Result);
                };
            }
            return;
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        _addressableHandles[path] = handle;

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"Failed to load asset from Addressables: {path}");
                onComplete?.Invoke(null);
                _addressableHandles.Remove(path);
            }
        };
    }

    private T LoadFromAddressables<T>(string path) where T : UnityEngine.Object
    {
        if (_addressableHandles.TryGetValue(path, out AsyncOperationHandle existingHandle))
        {
            if (existingHandle.IsDone)
            {
                return (T)existingHandle.Result;
            }
            else
            {
                Debug.LogWarning($"Synchronous load requested for async operation: {path}");
                existingHandle.WaitForCompletion();
                return (T)existingHandle.Result;
            }
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        _addressableHandles[path] = handle;
        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load asset from Addressables: {path}");
            _addressableHandles.Remove(path);
            return null;
        }
    }

    private void InstantiateFromAddressablesAsync(string path, Action<GameObject> onComplete, Transform parent = null)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(path, parent);
        _addressableHandles[path] = handle;

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"Failed to instantiate from Addressables: {path}");
                onComplete?.Invoke(null);
                _addressableHandles.Remove(path);
            }
        };
    }

    private GameObject InstantiateFromAddressables(string path, Transform parent = null)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(path, parent);
        _addressableHandles[path] = handle;
        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to instantiate from Addressables: {path}");
            _addressableHandles.Remove(path);
            return null;
        }
    }

    public void ReleaseAsset(string path)
    {
        if (_loadMode != ResourceLoadMode.Addressables) return;

        if (_addressableHandles.TryGetValue(path, out AsyncOperationHandle handle))
        {
            Addressables.Release(handle);
            _addressableHandles.Remove(path);
        }
    }

    public void ReleaseAllAddressables()
    {
        foreach (var handle in _addressableHandles.Values)
        {
            Addressables.Release(handle);
        }
        _addressableHandles.Clear();
    }

    #endregion
}

