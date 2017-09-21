using System.Runtime.InteropServices;
using UnityEngine;

public class LangfordAttractor : MonoBehaviour
{
    [SerializeField] float a = 1.0f;
    [SerializeField] float v = 0.7f;
    [SerializeField] float l = 0.6f;
    [SerializeField] float o = 3.5f;
    [SerializeField] float r = 0.25f;
    [SerializeField] float e = 0.0f;

    [SerializeField] int numberOfDraw = 200000;
    [SerializeField] int initialIterate = 0;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float particleScale = 0.1f;
    [SerializeField] Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 200.0f);

    [SerializeField] Mesh mesh;
    [SerializeField] Shader shader;
    [SerializeField] ComputeShader kernel;

    ComputeBuffer transformBuff;
    ComputeBuffer argsBuff;
    Material mat;

    void KernelUpdate(float dt)
    {
        kernel.SetFloat("_A", a);
        kernel.SetFloat("_V", v);
        kernel.SetFloat("_L", l);
        kernel.SetFloat("_O", o);
        kernel.SetFloat("_R", r);
        kernel.SetFloat("_E", e);
        kernel.SetFloat("_DeltaTime", dt);
        kernel.Dispatch(kernel.FindKernel("Calculate"), numberOfDraw / 8 + 1, 1, 1);
    }
    void OnDisable()
    {
        transformBuff.Release();
        argsBuff.Release();
        transformBuff = null;
        argsBuff = null;
        Destroy(mat);
    }
    void Start()
    {
        var data = new TransformStruct[numberOfDraw];
        for (var i = 0; i < numberOfDraw; i++)
        {
            data[i].translate = Vector3.one;
            data[i].scale = Vector3.one * particleScale;
            data[i].speed = Random.Range(0.1f, 1.0f);
            data[i].sumTime = i;
        }
        transformBuff = CreateComputeBuffer(data);
        var args = new uint[5] { 0, 0, 0, 0, 0 };
        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)numberOfDraw;
        argsBuff = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuff.SetData(args);
        kernel.SetBuffer(kernel.FindKernel("Calculate"), "_TransformBuff", transformBuff);

        mat = new Material(shader);
        mat.SetBuffer("_TransformBuff", transformBuff);

        for (int i = 0; i < initialIterate; i++)
        {
            KernelUpdate(1.0f / 60.0f);
        }
    }
    void Update()
    {
        KernelUpdate(Time.deltaTime * speed);
        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, bounds, argsBuff);
    }
    struct TransformStruct
    {
        public Vector3 translate;
        public Vector3 rotation;
        public Vector3 scale;
        public float sumTime;
        public float speed;
    }
    ComputeBuffer CreateComputeBuffer<T>(T[] data, ComputeBufferType type = ComputeBufferType.Default)
    {
        var computeBuffer = new ComputeBuffer(data.Length, Marshal.SizeOf(typeof(T)), type);
        computeBuffer.SetData(data);
        return computeBuffer;
    }
}