/// <summary>
/// レイヤー名を定数で管理するクラス
/// </summary>
public static class LayerName
{
	public const int Default = 0;
	public const int TransparentFX = 1;
	public const int IgnoreRaycast = 2;
	public const int Ground = 3;
	public const int Water = 4;
	public const int UI = 5;
	public const int Wall = 6;
	public const int Enemy = 7;
	public const int GrassCrusher = 8;
	public const int Player = 9;
	public const int DefaultMask = 1;
	public const int TransparentFXMask = 2;
	public const int IgnoreRaycastMask = 4;
	public const int GroundMask = 8;
	public const int WaterMask = 16;
	public const int UIMask = 32;
	public const int WallMask = 64;
	public const int EnemyMask = 128;
	public const int GrassCrusherMask = 256;
	public const int PlayerMask = 512;
}
