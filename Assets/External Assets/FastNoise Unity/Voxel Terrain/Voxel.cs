namespace VoxelEngine
{
	// Basic voxel data structure
	// More data could be added here like block types
	public struct Voxel
	{
		public static readonly Voxel solid = new Voxel(1f);
		public static readonly Voxel empty = new Voxel(-1f);

		public float density;

		public Voxel(float density = -1.0f)
		{
			this.density = density;
		}

		public bool IsSolid()
		{
			return density >= 0;
		}
	}
}
