using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaStorageManager : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private List<NakamaCollectionObject> autoLoadObjects;

        private NakamaManager nakamaManager = null;

        #endregion

        #region EVENTS

        public event Action onLoaded = null;

        #endregion

        #region PROPERTIES

        public static NakamaStorageManager Instance { get; private set; }
        public bool LoadingFinished { get; private set; } = false;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaManager.onLoginSuccess += AutoLoad;
        }

        private void OnDestroy()
        {
            nakamaManager.onLoginSuccess -= AutoLoad;
        }

        private void AutoLoad()
        {
            UpdateCollectionObjects(autoLoadObjects, Loaded);
        }

        private void Loaded(bool result)
        {
            LoadingFinished = true;
            onLoaded?.Invoke();
        }

        private void UpdateCollectionObject(NakamaCollectionObject collectionObject, Action<bool> onResult = null)
        {
            UpdateCollectionObjectsAsync(new List<NakamaCollectionObject>() { collectionObject });
        }

        private void UpdateCollectionObjects(IEnumerable<NakamaCollectionObject> collectionObjects, Action<bool> onResult = null)
        {
            UpdateCollectionObjectsAsync(collectionObjects, onResult);
        }

        private async void UpdateCollectionObjectsAsync(IEnumerable<NakamaCollectionObject> collectionObjects, Action<bool> onResult = null)
        {
            if (collectionObjects.Count() == default(int))
            {
                onResult?.Invoke(false);
                return;
            }

            List<IApiReadStorageObjectId> storageObjectIds = new List<IApiReadStorageObjectId>();
            foreach (NakamaCollectionObject collectionObject in collectionObjects)
            {
                collectionObject.ResetData();
                StorageObjectId storageObjectId = new StorageObjectId
                {
                    Collection = collectionObject.Collection,
                    Key = collectionObject.Key,
                    UserId = nakamaManager.Session.UserId
                };

                storageObjectIds.Add(storageObjectId);
            }

            var result = await nakamaManager.Client.ReadStorageObjectsAsync(nakamaManager.Session, storageObjectIds.ToArray<IApiReadStorageObjectId>());
            foreach (IApiStorageObject storageObject in result.Objects)
            {
                foreach (NakamaCollectionObject collectionObject in collectionObjects)
                {
                    if (storageObject.Key != collectionObject.Key)
                        continue;

                    collectionObject.SetDatabaseValue(storageObject.Value, storageObject.Version);
                }
            }

            onResult?.Invoke(true);
        }

        public async void SendValueToServer(NakamaCollectionObject collectionObject, object newValue, Action<bool> onResult = null)
        {
            WriteStorageObject writeStorageObject = new WriteStorageObject
            {
                Collection = collectionObject.Collection,
                Key = collectionObject.Key,
                Value = newValue.Serialize(),
                Version = collectionObject.Version
            };

            try
            {
                var objectIds = await nakamaManager.Client.WriteStorageObjectsAsync(nakamaManager.Session, new WriteStorageObject[] { writeStorageObject });
                foreach (IApiStorageObjectAck storageObject in objectIds.Acks)
                {
                    if (storageObject.Key != collectionObject.Key)
                        continue;

                    collectionObject.SetDatabaseValue(newValue.Serialize(), storageObject.Version);
                }

                onResult?.Invoke(true);
            }
            catch
            {
                UpdateCollectionObject(collectionObject, (result) => onResult?.Invoke(false));
            }
        }

        #endregion
    }
}
