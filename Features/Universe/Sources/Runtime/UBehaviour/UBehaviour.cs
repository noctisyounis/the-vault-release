using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universe.SceneTask.Runtime;
using Sirenix.OdinInspector;
using static Universe.SceneTask.Runtime.Task;
using static Universe.UFile;

namespace Universe
{
    public abstract class UBehaviour : SerializedMonoBehaviour
    {
        #region Public

        public static bool MasterDebug;
        public bool IsMasterDebug => MasterDebug;
        
        [Header("Debug")] 
        public bool IsDebug;
        public bool IsVerbose;
        public bool GizmosVisible;

        [Header("Life time")]
        public bool UseUpdates = true;
        
        #endregion
        
        
        #region Universe Core Loop

        public virtual void Awake()
        {
            Task.RegisterUpdate(this);
            Task.RegisterFixedUpdate(this);
            Task.RegisterLateUpdate(this);
        }

        public virtual void OnDestroy()
        {
            Task.UnregisterUpdate(this);
            Task.UnregisterFixedUpdate(this);
            Task.UnregisterLateUpdate(this);
        }

        public virtual void OnUpdate(float deltatime) {}
        public virtual void OnFixedUpdate(float fixedDeltaTime) {}
        public virtual void OnLateUpdate(float deltaTime) {}

        #endregion
        
        
        #region Spawn

        protected void Spawn(AssetReference assetReference, int poolSize = 0) => 
            this.USpawn(assetReference, poolSize);

        protected void Spawn( AssetReference assetReference, Transform parent, Action<GameObject> callback, int poolSize = 0 ) =>
            this.USpawn( assetReference, parent, callback, poolSize); 
        
        protected void Spawn(AssetReference assetReference, Vector3 pos, Quaternion rotation, int poolSize = 0 ) =>
            this.USpawn(assetReference, pos, rotation, poolSize);

        protected void Spawn( AssetReference assetReference, Vector3 pos, Quaternion rotation, Vector3 scale, int poolSize = 0 ) =>
            this.USpawn( assetReference, pos, rotation, scale, poolSize );

        protected void Spawn(AssetReference assetReference, Vector3 pos, Quaternion rotation, Transform parent, int poolSize = 0) =>
            this.USpawn(assetReference, pos, rotation, parent, poolSize);

        protected void Spawn( AssetReference assetReference, Vector3 pos, Quaternion rotation, Vector3 scale, Transform parent, int poolSize = 0 ) =>
            this.USpawn( assetReference, pos, rotation, scale, parent, poolSize );

        protected void Spawn(AssetReference assetReference, Vector3 pos, Quaternion rotation, Action<GameObject> callback, int poolSize = 0) => 
            this.USpawn(assetReference, pos, rotation, callback, poolSize);

        protected void Spawn( AssetReference assetReference, Vector3 pos, Quaternion rotation, Vector3 scale, Action<GameObject> callback, int poolSize = 0 ) =>
            this.USpawn( assetReference, pos, rotation, scale, callback, poolSize );

        protected void Spawn(AssetReference assetReference, Vector3 pos, Quaternion rotation, Transform parent, Action<GameObject> callback, int poolSize = 0) => 
            this.USpawn(assetReference, pos, rotation, parent, callback, poolSize);

        protected void Spawn( AssetReference assetReference, Vector3 pos, Quaternion rotation, Vector3 scale, Transform parent, Action<GameObject> callback, int poolSize = 0 ) =>
            this.USpawn( assetReference, pos, rotation, scale, parent, callback, poolSize );

        #endregion


        #region Despawn

        protected void Despawn() =>
            this.UDespawn();

        protected void Despawn( GameObject target ) =>
            this.UDespawn( target );

        #endregion


        #region Verbose

        [Conditional("DEBUG")]
        protected void Verbose(string message) => 
            this.ULog(message);

        [Conditional("DEBUG")]
        protected void Verbose<T>(string message, T val) => 
            this.ULog(message, val);
        
        [Conditional("DEBUG")]
        protected void Verbose<T>(string message, List<T> values) => 
            this.ULog(message, values);
        
        [Conditional("DEBUG")]
        protected void Verbose<T>(string message, IEnumerable<T> values) => 
            this.ULog(message, values);

        [Conditional("DEBUG")]
        protected void Verbose<T>(string message, Dictionary<T, T> values) => 
            this.ULog(message, values);

        [Conditional("DEBUG")]
        protected void VerboseWarning(string message) =>
            this.UWarning(message);

        [Conditional("DEBUG")]
        protected void VerboseError(string message) =>
            this.UError(message);

        #endregion
        
        
        #region Gizmos

        public void DrawCube(Vector3 position, float size, Color color) =>
            this.UDrawCube(position, Quaternion.identity, size, color);
        
        public void DrawCube(Vector3 position, Quaternion rotation, float size, Color color) =>
            this.UDrawCube(position, rotation, size, color);

        public void DrawCuboid(Vector3 position, Quaternion rotation, Vector3 size, Color color) =>
            this.UDrawCuboid(position, rotation, size, color);

        public void DrawLine(Vector3 start, Vector3 end, Color color) =>
            this.UDrawLine(start, end, color);

        public void DrawSphere(Vector3 position, float radius, Color color) =>
            this.UDrawSphere(position, radius, color);

        public void DrawTorus(Vector3 position, Quaternion rotation, float radius, float thickness, Color color) =>
            this.UDrawTorus(position, rotation, radius, thickness, color);

        public void DrawCone(Vector3 position, Quaternion rotation, float radius, float length, Color color) =>
            this.UDrawCone(position, rotation, radius, length, color);

        public void DrawPolyline(List<Vector3> points, bool closed, float thickness, Color color) =>
            this.UDrawPolyline(points, closed, thickness, color);

        public void DrawPolygon(List<Vector3> points, Color color) =>
            this.UDrawPolygon(points, color);

        public void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, float roundness, Color color) =>
            this.UDrawTriangle(a, b, c, roundness, color);

        public void DrawDisc(Vector3 position, Vector3 normal, float radius, Color color) =>
            this.UDrawDisc(position, normal, radius, color);

        public void DrawRectangle(Vector3 position, Quaternion rotation, Vector2 size, Color color) =>
            this.UDrawRectangle(position, rotation, size, color);
        
        #endregion
        

        #region Inputs

        protected float GetAxis(string axis) => 
            this.UGetAxis(axis);

        protected bool GetKey(UKeyCode keyCode) => 
            this.UGetKey(keyCode);

        protected bool GetKeyDown(UKeyCode keyCode) => 
            this.UGetKeyDown(keyCode);
        
        protected bool GetKeyUp(UKeyCode keyCode) => 
            this.UGetKeyUp(keyCode);

        #endregion


        #region Checkpoint

        protected void SaveCheckpoint() =>
            this.USaveCheckpoint();
        protected void SaveCheckpoint(CheckpointData checkpoint) =>
            this.USaveCheckpoint(checkpoint);
        protected void SaveCheckpoint(CheckpointData checkpoint, LevelData level, SituationData situation) =>
            this.USaveCheckpoint(checkpoint, level, situation);

        protected void LoadCheckpoint(LoadLevelMode mode = LoadLevelMode.LoadAll) =>
            this.ULoadCheckpoint(mode);

        protected void LoadCheckpoint(CheckpointData checkpoint, LoadLevelMode mode = LoadLevelMode.LoadAll) =>
            this.ULoadCheckpoint(checkpoint, mode);

        protected void ReloadCheckpoint(LoadLevelMode mode = LoadLevelMode.LoadAll) =>
            this.UReloadCheckpoint(mode);

        #endregion


        #region Level

        protected void LoadLevel( LevelData level, SituationData situation = null) =>
            this.ULoadLevel( level, situation );

        protected void ChangeLevel( LevelData level, SituationData situation = null, LoadLevelMode mode = LoadLevelMode.LoadAll ) =>
            this.UChangeLevel( level, situation, mode );

        protected void ReloadLevel( LoadLevelMode mode = LoadLevelMode.LoadAll ) =>
            this.UReloadLevel( mode );       
        
        protected void UnloadLevel(LevelData level) =>
            this.UUnloadLevel(level);

        #endregion
        
        
        #region Situation
        
        protected void LoadSituation( SituationData situation ) =>
            this.ULoadSituation( situation );

        protected void UnloadSituation( SituationData situation ) =>
            this.UUnloadSituation( situation );

        #endregion


        #region Tasks

        protected void LoadTask( TaskData task ) =>
            this.ULoadTask( task );

        protected void UnloadLastTaskAndLoad( TaskData task ) =>
            this.UUnloadLastTaskAndLoad( task );

        protected void UnloadTask( TaskData task ) =>
            this.UUnloadTask( task );

        #endregion


        #region UFile

        public string GetOrCreateFolderAt( string path ) =>
            UGetOrCreateFolderAt( path );

        public string GetPathRelativeToProject( string path ) =>
            UGetPathRelativeToProject( path );

        public string RemoveAssetsPathFrom( string path ) =>
            URemoveAssetsPathFrom( path );

        #endregion


        #region Cached Members
#pragma warning disable 0109

        [NonSerialized]
        private Transform _transform;
        public new Transform transform => _transform ? _transform : _transform = GetComponent<Transform>();
      
        [NonSerialized]
        private Animation _animation;
        public new Animation animation => 
            _animation ? _animation : _animation = GetComponent<Animation>();
        
        [NonSerialized]
        private Camera _camera;
        public new Camera camera => 
            _camera ? _camera : _camera = GetComponent<Camera>();

        [NonSerialized]
        private Collider _collider;
        public new Collider collider => 
            _collider ? _collider : _collider = GetComponent<Collider>();

        [NonSerialized]
        private Collider2D _collider2D;
        public new Collider2D collider2D => 
            _collider2D ? _collider2D : _collider2D = GetComponent<Collider2D>();

        [NonSerialized]
        private ConstantForce _constantForce;
        public new ConstantForce constantForce => 
            _constantForce ? _constantForce : _constantForce = GetComponent<ConstantForce>();

        [NonSerialized]
        private HingeJoint _hingeJoint;
        public new HingeJoint hingeJoint => 
            _hingeJoint ? _hingeJoint : _hingeJoint = GetComponent<HingeJoint>();

        [NonSerialized]
        private Light _light;
        public new Light light => 
            _light ? _light : _light = GetComponent<Light>();

        [NonSerialized]
        private ParticleSystem _particleSystem;
        public new ParticleSystem particleSystem => 
            _particleSystem ? _particleSystem : _particleSystem = GetComponent<ParticleSystem>();

        [NonSerialized]
        private Renderer _renderer;
        public new Renderer renderer => 
            _renderer ? _renderer : _renderer = GetComponent<Renderer>();

        [NonSerialized]
        private Rigidbody _rigidbody;
        public new Rigidbody rigidbody => 
            _rigidbody ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        [NonSerialized]
        private Rigidbody2D _rigidbody2D;
        public new Rigidbody2D rigidbody2D => 
            _rigidbody2D ? _rigidbody2D : _rigidbody2D = GetComponent<Rigidbody2D>();

        #pragma warning restore 0109
#endregion
    }
}
