using System;
using System.Runtime.InteropServices;
using UnityEngine;

using POINTER = System.UInt64;

public class FastNoiseSIMD
{
	public enum NoiseType
	{
		Value,
		ValueFractal,
		Perlin,
		PerlinFractal,
		Simplex,
		SimplexFractal,
		WhiteNoise,
		Cellular
	};

	public enum FractalType
	{
		FBM,
		Billow,
		RigidMulti
	};

	public enum CellularDistanceFunction
	{
		Euclidean,
		Manhattan,
		Natural
	};

	public enum CellularReturnType
	{
		CellValue,
		Distance,
		Distance2,
		Distance2Add,
		Distance2Sub,
		Distance2Mul,
		Distance2Div
	};

	private readonly POINTER nativePointer;

	public FastNoiseSIMD(int seed = 1337)
	{
		nativePointer = NewFastNoiseSIMD(seed);
	}

	~FastNoiseSIMD()
	{
		NativeFree(nativePointer);
	}

	// Returns seed used for all noise types
	public int GetSeed()
	{
		return NativeGetSeed(nativePointer);
	}

	// Sets seed used for all noise types
	// Default: 1337
	public void SetSeed(int seed)
	{
		NativeSetSeed(nativePointer, seed);
	}

	// Sets frequency for all noise types
	// Default: 0.01
	public void SetFrequency(float frequency)
	{
		NativeSetFrequency(nativePointer, frequency);
	}

	// Sets noise return type of (Get/Fill)NoiseSet()
	// Default: Simplex
	public void SetNoiseType(NoiseType noiseType)
	{
		NativeSetNoiseType(nativePointer, (int)noiseType);
	}

	// Sets scaling factor for individual axis
	// Defaults: 1.0
	public void SetAxisScales(float xScale, float yScale, float zScale)
	{
		NativeSetAxisScales(nativePointer, xScale, yScale, zScale);
	}


	// Sets octave count for all fractal noise types
	// Default: 3
	public void SetFractalOctaves(int octaves)
	{
		NativeSetFractalOctaves(nativePointer, octaves);
	}

	// Sets octave lacunarity for all fractal noise types
	// Default: 2.0
	public void SetFractalLacunarity(float lacunarity)
	{
		NativeSetFractalLacunarity(nativePointer, lacunarity);
	}

	// Sets octave gain for all fractal noise types
	// Default: 0.5
	public void SetFractalGain(float gain)
	{
		NativeSetFractalGain(nativePointer, gain);
	}

	// Sets method for combining octaves in all fractal noise types
	// Default: FBM
	public void SetFractalType(FractalType fractalType)
	{
		NativeSetFractalType(nativePointer, (int)fractalType);
	}


	// Sets return type from cellular noise calculations
	// Default: Distance
	public void SetCellularReturnType(CellularReturnType cellularReturnType)
	{
		NativeSetCellularReturnType(nativePointer, (int)cellularReturnType);
	}

	// Sets distance function used in cellular noise calculations
	// Default: Euclidean
	public void SetCellularDistanceFunction(CellularDistanceFunction cellularDistanceFunction)
	{
		NativeSetCellularDistanceFunction(nativePointer, (int)cellularDistanceFunction);
	}

	public void FillNoiseSet(float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, float scaleModifier = 1.0f)
	{
		NativeFillNoiseSet(nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, scaleModifier);
	}

	public void FillSampledNoiseSet(float[] noiseSet, int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, int sampleScale)
	{
		NativeFillSampledNoiseSet(nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, sampleScale);
	}

	public void FillNoiseSetVector(float[] noiseSet, VectorSet vectorSet, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
	{
		NativeFillNoiseSetVector(nativePointer, noiseSet, vectorSet.nativePointer, xOffset, yOffset, zOffset);
	}

	public void FillSampledNoiseSetVector(float[] noiseSet, VectorSet vectorSet, int xSize, int ySize, int zSize, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
	{
		NativeFillSampledNoiseSetVector(nativePointer, noiseSet, vectorSet.nativePointer, xSize, ySize, zSize, xOffset, yOffset, zOffset);
	}

	public float[] GetNoiseSet(int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, float scaleModifier = 1.0f)
	{
		float[] noiseSet = GetEmptyNoiseSet(xSize, ySize, zSize);
		NativeFillNoiseSet(nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, scaleModifier);
		return noiseSet;
	}

	public float[] GetSampledNoiseSet(int xStart, int yStart, int zStart, int xSize, int ySize, int zSize, int sampleScale)
	{
		float[] noiseSet = GetEmptyNoiseSet(xSize, ySize, zSize);
		NativeFillSampledNoiseSet(nativePointer, noiseSet, xStart, yStart, zStart, xSize, ySize, zSize, sampleScale);
		return noiseSet;
	}

	public float[] GetEmptyNoiseSet(int xSize, int ySize, int zSize)
	{
		return new float[xSize * ySize * zSize];
	}

#if UNITY_STANDALONE || UNITY_EDITOR
	private const string NATIVE_LIB = "FastNoiseSIMD_CLib";

	[DllImport(NATIVE_LIB)]
	public static extern int GetSIMDLevel();

	[DllImport(NATIVE_LIB)]
	public static extern void SetSIMDLevel(int level);

	[DllImport(NATIVE_LIB)]
	private static extern POINTER NewFastNoiseSIMD(int seed);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeFree(POINTER nativePointer);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetSeed(POINTER nativePointer, int seed);

	[DllImport(NATIVE_LIB)]
	private static extern int NativeGetSeed(POINTER nativePointer);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetFrequency(POINTER nativePointer, float freq);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetNoiseType(POINTER nativePointer, int noiseType);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetAxisScales(POINTER nativePointer, float xScale, float yScale, float zScale);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetFractalOctaves(POINTER nativePointer, int octaves);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetFractalLacunarity(POINTER nativePointer, float lacunarity);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetFractalGain(POINTER nativePointer, float gain);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetFractalType(POINTER nativePointer, int fractalType);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetCellularDistanceFunction(POINTER nativePointer, int distanceFunction);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeSetCellularReturnType(POINTER nativePointer, int returnType);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeFillNoiseSet(POINTER nativePointer, float[] noiseSet, int xStart, int yStart, int zStart,
		int xSize, int ySize, int zSize, float scaleModifier);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeFillSampledNoiseSet(POINTER nativePointer, float[] noiseSet, int xStart, int yStart, int zStart,
		int xSize, int ySize, int zSize, int sampleScale);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeFillNoiseSetVector(POINTER nativePointer, float[] noiseSet, POINTER vectorSetPointer,
		float xOffset, float yOffset, float zOffset);

	[DllImport(NATIVE_LIB)]
	private static extern void NativeFillSampledNoiseSetVector(POINTER nativePointer, float[] noiseSet, POINTER vectorSetPointer,
		int xSize, int ySize, int zSize, float xOffset, float yOffset, float zOffset);

#else
	public static int GetSIMDLevel() { return -2; }

	public static void SetSIMDLevel(int level) { }

	private static POINTER NewFastNoiseSIMD(int seed)
	{
		Debug.LogError("FastNoise SIMD not supported on this platform");
		return 0;
	}
	
	private static void NativeFree(POINTER nativePointer) { }

	private static void NativeSetSeed(POINTER nativePointer, int seed) { }

	private static int NativeGetSeed(POINTER nativePointer) { return 0; }

	private static void NativeSetFrequency(POINTER nativePointer, float freq) { }

	private static void NativeSetNoiseType(POINTER nativePointer, int noiseType) { }

	private static void NativeSetAxisScales(POINTER nativePointer, float xScale, float yScale, float zScale) { }

	private static void NativeSetFractalOctaves(POINTER nativePointer, int octaves) { }

	private static void NativeSetFractalLacunarity(POINTER nativePointer, float lacunarity) { }

	private static void NativeSetFractalGain(POINTER nativePointer, float gain) { }

	private static void NativeSetFractalType(POINTER nativePointer, int fractalType) { }

	private static void NativeSetCellularDistanceFunction(POINTER nativePointer, int distanceFunction) { }

	private static void NativeSetCellularReturnType(POINTER nativePointer, int returnType) { }

	private static void NativeFillNoiseSet(POINTER nativePointer, float[] noiseSet, int xStart, int yStart, int zStart,
		int xSize, int ySize, int zSize, float scaleModifier) { }

	private static void NativeFillSampledNoiseSet(POINTER nativePointer, float[] noiseSet, int xStart, int yStart,
		int zStart, int xSize, int ySize, int zSize, int sampleScale) { }
	
	private static void NativeFillNoiseSetVector(POINTER nativePointer, float[] noiseSet, POINTER vectorSetPointer,
		float xOffset, float yOffset, float zOffset) { }
	
	private static void NativeFillSampledNoiseSetVector(POINTER nativePointer, float[] noiseSet, POINTER vectorSetPointer,
		int xSize, int ySize, int zSize, float xOffset, float yOffset, float zOffset) { }
#endif

	public class VectorSet
	{
		internal readonly POINTER nativePointer;

		public VectorSet(Vector3[] vectors, int samplingScale = 0)
		{
			float[] vectorSetArray = new float[vectors.Length*3];

			for (int i = 0; i < vectors.Length; i++)
			{
				vectorSetArray[i] = vectors[i].x;
				vectorSetArray[i+vectors.Length] = vectors[i].y;
				vectorSetArray[i+vectors.Length*2] = vectors[i].z;
			}

			nativePointer = NewVectorSet(vectorSetArray, vectorSetArray.Length, samplingScale);
		}

		~VectorSet()
		{
			NativeFreeVectorSet(nativePointer);
		}
		
#if UNITY_STANDALONE || UNITY_EDITOR
		[DllImport(NATIVE_LIB)]
		private static extern POINTER NewVectorSet(float[] vectorSetArray, int arraySize, int samplingScale);

		[DllImport(NATIVE_LIB)]
		private static extern void NativeFreeVectorSet(POINTER nativePointer);
#else
		private static POINTER NewVectorSet(float[] vectorSetArray, int arraySize, int samplingScale = 0)
		{
			Debug.LogError("FastNoise SIMD Vector Set not supported on this platform");
			return 0;
		}
		
		private static void NativeFreeVectorSet(POINTER nativePointer) { }
#endif
	}
}
