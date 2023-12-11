using System.Numerics;
using System.Security.AccessControl;
using Shared;
using Core;
using Core.Animation;
using Core.PlayerLoop;
using Core.PlayerLoopStages;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Examples;

public class Example2
{
    readonly Surface _surface;
    readonly PlayerLoop _playerLoop;

    readonly Node _camera;

    // readonly Node _cameraRoot;
    readonly List<Node> _renderers = new List<Node>();

    readonly Vertex[] _vertices =
    {
        new() { _pos = new Vector3D<float>(0.2878271f, -0.2878271f, -0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(0.2878271f, -0.2878271f, 0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(-0.2878271f, -0.2878271f, -0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(-0.2878271f, -0.2878271f, 0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(0.2888354f, 0.2879004f, -0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.683609f) },
        new() { _pos = new Vector3D<float>(0.2888354f, 0.2879004f, 0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.683609f) },
        new() { _pos = new Vector3D<float>(-0.2888354f, 0.2879004f, -0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6836091f) },
        new() { _pos = new Vector3D<float>(-0.2888354f, 0.2879004f, 0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.683609f) },
        new() { _pos = new Vector3D<float>(0, -0.5f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3734581f, 0.02175571f) },
        new() { _pos = new Vector3D<float>(4.764297E-09f, 0.5f, -4.768371E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3847829f, 0.9717647f) },
        new() { _pos = new Vector3D<float>(0.5f, -4.764297E-09f, -1.639128E-07f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(8.19558E-08f, -9.537071E-09f, 0.5f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.5f, 4.764297E-09f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-2.235174E-07f, 3.274181E-13f, -0.5f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-5.340553E-08f, -0.3525147f, -0.3525147f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(-0.3525147f, -0.3525147f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(5.722046E-08f, -0.3525147f, 0.3525147f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(0.3525147f, -0.3525147f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(0.3536914f, 0.3527881f, -1.144409E-07f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(5.722046E-08f, 0.3527881f, 0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(-0.3536914f, 0.3527881f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(-1.573563E-07f, 0.3527881f, -0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(0.3535523f, -3.128662E-11f, 0.3535523f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.3536914f, -0.0008836466f, -0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.3535523f, 3.128662E-11f, 0.3535523f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.3536914f, -0.0008836116f, -0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.2401929f, -0.3089014f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3540024f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.1701416f, -0.3313476f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3243351f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.0880603f, -0.3469702f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2884209f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.2401929f, -0.3089014f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1437824f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(-0.1701416f, -0.3313476f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1734497f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.0880603f, -0.3469702f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2093638f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.3089014f, -0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1041886f, 0.3048139f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.3313476f, -0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.07452133f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.3469702f, -0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03860712f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.3089014f, 0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8932236f, 0.3048139f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.3313476f, 0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.922891f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.3469702f, 0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9588053f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.2401929f, -0.3089014f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8536299f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(-0.1701416f, -0.3313476f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8239627f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.0880603f, -0.3469702f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7880484f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.2401929f, -0.3089014f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6434099f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.1701416f, -0.3313476f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6730773f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.0880603f, -0.3469702f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7089914f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.3089014f, 0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6038162f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.3313476f, 0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5741488f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.3469702f, 0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5382347f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.3089014f, -0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3935962f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.3313476f, -0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4232635f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.3469702f, -0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4591776f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.3090894f, -0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3935962f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.3315698f, -0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4232635f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.3472266f, -0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4591776f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.3090894f, 0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6038162f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.3315698f, 0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5741488f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.3472266f, 0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5382347f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(0.2409058f, 0.3090894f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6434099f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.1707129f, 0.3315698f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6730773f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.08835449f, 0.3472266f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7089914f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.2409058f, 0.3090894f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8536299f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.1707129f, 0.3315698f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8239627f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.08835449f, 0.3472266f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7880484f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.3090894f, 0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8932238f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.3315698f, 0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.922891f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.3472266f, 0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9588053f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.3090894f, -0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1041886f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.3315698f, -0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.07452133f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.3472266f, -0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03860712f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.2409058f, 0.3090894f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1437824f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.1707129f, 0.3315698f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1734497f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.08835449f, 0.3472266f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2093638f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(0.2409058f, 0.3090894f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3540024f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.1707129f, 0.3315698f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3243351f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.08835449f, 0.3472266f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2884209f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.2401929f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.348428f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.1701416f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.0880603f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.2401733f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.1698926f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.08750671f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.5568f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.2401733f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.1698926f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.08750671f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.5568f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.2401929f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.348428f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.1701416f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.0880603f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.2401929f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.348428f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.1701416f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.0880603f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.2401733f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.1698926f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.08750671f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.5568f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.2401929f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.348428f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.1701416f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.0880603f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.2401733f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.1698926f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.08750671f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.5568f) },
        new() { _pos = new Vector3D<float>(-1.811975E-08f, -0.4123633f, -0.2801587f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.233575f) },
        new() { _pos = new Vector3D<float>(-3.91007E-08f, -0.4585547f, -0.1956055f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.1743396f) },
        new() { _pos = new Vector3D<float>(-1.182554E-08f, -0.4882324f, -0.1007947f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.1020471f) },
        new() { _pos = new Vector3D<float>(-0.2801587f, -0.4123633f, 1.907349E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.233575f) },
        new() { _pos = new Vector3D<float>(-0.1956055f, -0.4585547f, -3.814694E-10f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.1733436f) },
        new() { _pos = new Vector3D<float>(-0.1007953f, -0.4882324f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.1111187f) },
        new() { _pos = new Vector3D<float>(4.768371E-08f, -0.4123633f, 0.2801587f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.233575f) },
        new() { _pos = new Vector3D<float>(-2.002722E-08f, -0.4585547f, 0.1956055f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.1743396f) },
        new() { _pos = new Vector3D<float>(-2.136236E-08f, -0.4882324f, 0.1007947f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.1046903f) },
        new() { _pos = new Vector3D<float>(0.2801587f, -0.4123633f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.233575f) },
        new() { _pos = new Vector3D<float>(0.1956055f, -0.4585547f, 1.945489E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.1695687f) },
        new() { _pos = new Vector3D<float>(0.1007947f, -0.4882324f, -9.536743E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.09756867f) },
        new() { _pos = new Vector3D<float>(0.2811499f, 0.4128613f, -9.059906E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.7626636f) },
        new() { _pos = new Vector3D<float>(0.1962756f, 0.4591016f, -6.198883E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.8162426f) },
        new() { _pos = new Vector3D<float>(0.1011365f, 0.4888574f, -3.218651E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.9020402f) },
        new() { _pos = new Vector3D<float>(4.768371E-08f, 0.4128613f, 0.2811499f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.7626636f) },
        new() { _pos = new Vector3D<float>(3.099441E-08f, 0.4591016f, 0.1962756f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.8162426f) },
        new() { _pos = new Vector3D<float>(1.549721E-08f, 0.4888574f, 0.1011359f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.748657f, 0.9019565f) },
        new() { _pos = new Vector3D<float>(-0.2811499f, 0.4128613f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.7626636f) },
        new() { _pos = new Vector3D<float>(-0.1962756f, 0.4591016f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.8162426f) },
        new() { _pos = new Vector3D<float>(-0.1011359f, 0.4888574f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9978603f, 0.8990951f) },
        new() { _pos = new Vector3D<float>(-1.239777E-07f, 0.4128613f, -0.2811499f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.7626636f) },
        new() { _pos = new Vector3D<float>(-8.583068E-08f, 0.4591016f, -0.1962756f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.8162426f) },
        new() { _pos = new Vector3D<float>(-4.410744E-08f, 0.4888574f, -0.1011359f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.9020402f) },
        new() { _pos = new Vector3D<float>(0.4123633f, -0.2801587f, -3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.3230592f) },
        new() { _pos = new Vector3D<float>(0.4585547f, -0.1956055f, -7.667521E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.3767318f) },
        new() { _pos = new Vector3D<float>(0.4882324f, -0.1007953f, -3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.4369167f) },
        new() { _pos = new Vector3D<float>(0.4157349f, 2.270099E-11f, 0.2777856f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5936133f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.4619385f, 3.492459E-11f, 0.1913415f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.56283f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.4903906f, 7.275958E-12f, 0.09754516f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5310841f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.4137475f, 0.2802221f, -1.335144E-07f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.6787412f) },
        new() { _pos = new Vector3D<float>(0.4598437f, 0.195459f, -1.525879E-07f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.6250687f) },
        new() { _pos = new Vector3D<float>(0.4896582f, 0.1002966f, -1.621246E-07f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.5648838f) },
        new() { _pos = new Vector3D<float>(0.4137475f, -0.0008836472f, -0.2811499f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.403799f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.4598437f, -0.0008836464f, -0.1962756f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4345824f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.4896582f, -0.0008836508f, -0.1011359f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4663283f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(6.67572E-08f, -0.2801587f, 0.4123633f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.3230592f) },
        new() { _pos = new Vector3D<float>(7.571769E-08f, -0.1956055f, 0.4585547f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.3767318f) },
        new() { _pos = new Vector3D<float>(8.038161E-08f, -0.1007953f, 0.4882324f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.4369167f) },
        new() { _pos = new Vector3D<float>(-0.2777856f, 3.143213E-11f, 0.4157349f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8434271f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.1913415f, -8.076313E-12f, 0.4619385f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8126438f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.09754516f, 3.588866E-11f, 0.4903906f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7808979f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(6.67572E-08f, 0.2802221f, 0.4137475f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.6787412f) },
        new() { _pos = new Vector3D<float>(7.629394E-08f, 0.195459f, 0.4598437f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.6250687f) },
        new() { _pos = new Vector3D<float>(7.629394E-08f, 0.1002972f, 0.4896582f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.5648838f) },
        new() { _pos = new Vector3D<float>(0.2777856f, -3.143213E-11f, 0.4157349f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6536129f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.1913415f, 8.076313E-12f, 0.4619385f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6843961f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.09754516f, -3.605237E-11f, 0.4903906f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7161421f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.4123633f, -0.2801587f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.3230592f) },
        new() { _pos = new Vector3D<float>(-0.4585547f, -0.1956055f, 3.852852E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.3767318f) },
        new() { _pos = new Vector3D<float>(-0.4882324f, -0.1007947f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.4369167f) },
        new() { _pos = new Vector3D<float>(-0.4137475f, -0.0008836098f, -0.2811499f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09398577f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.4598437f, -0.0008836057f, -0.1962756f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06320244f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.4896582f, -0.000883606f, -0.1011359f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03145653f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.4137475f, 0.2802221f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.6787412f) },
        new() { _pos = new Vector3D<float>(-0.4598437f, 0.195459f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.6250687f) },
        new() { _pos = new Vector3D<float>(-0.4896582f, 0.1002972f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.5648838f) },
        new() { _pos = new Vector3D<float>(-0.4157349f, -2.313754E-11f, 0.2777856f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9034266f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.4619385f, -3.492459E-11f, 0.1913415f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9342099f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.4903906f, -7.275958E-12f, 0.09754516f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9659559f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(2.593995E-08f, -0.2801587f, -0.4123633f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.3230592f) },
        new() { _pos = new Vector3D<float>(-8.392307E-09f, -0.1956055f, -0.4585547f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.3767318f) },
        new() { _pos = new Vector3D<float>(3.356923E-08f, -0.1007953f, -0.4882324f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.4369167f) },
        new() { _pos = new Vector3D<float>(0.2811499f, -0.0008836342f, -0.4137475f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3437995f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.1962756f, -0.0008836379f, -0.4598437f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3130162f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.1011359f, -0.0008836288f, -0.4896582f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2812703f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-1.811981E-07f, 0.2802221f, -0.4137475f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.6787412f) },
        new() { _pos = new Vector3D<float>(-2.098083E-07f, 0.195459f, -0.4598437f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.6250687f) },
        new() { _pos = new Vector3D<float>(-2.193451E-07f, 0.1002972f, -0.4896582f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.5648838f) },
        new() { _pos = new Vector3D<float>(-0.2811499f, -0.0008836147f, -0.4137475f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1539853f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.1962756f, -0.0008836184f, -0.4598437f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1847686f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.1011359f, -0.0008836251f, -0.4896582f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2165145f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.2485034f, -0.3535864f, -0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.2834f) },
        new() { _pos = new Vector3D<float>(0.1748779f, -0.3852368f, -0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3420187f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(0.0902124f, -0.4055811f, -0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2992194f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(0.2637085f, -0.3852368f, -0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4055798f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(0.1839429f, -0.4252783f, -0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.2142188f) },
        new() { _pos = new Vector3D<float>(0.09450744f, -0.4502636f, -0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3216372f, 0.1856271f) },
        new() { _pos = new Vector3D<float>(0.2754956f, -0.4055811f, -0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4483792f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(0.1919995f, -0.4502636f, -0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4259613f, 0.1752676f) },
        new() { _pos = new Vector3D<float>(0.09875244f, -0.4785693f, -0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3725803f, 0.1336257f) },
        new() { _pos = new Vector3D<float>(-0.2485034f, -0.3535864f, -0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.2834f) },
        new() { _pos = new Vector3D<float>(-0.2637085f, -0.3852368f, -0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09220496f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(-0.2754956f, -0.4055811f, -0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.04940557f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(-0.1748779f, -0.3852368f, -0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.155766f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(-0.1839429f, -0.4252783f, -0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.2096534f) },
        new() { _pos = new Vector3D<float>(-0.1919995f, -0.4502636f, -0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.07182345f, 0.1859203f) },
        new() { _pos = new Vector3D<float>(-0.0902124f, -0.4055811f, -0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1985654f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(-0.09450744f, -0.4502636f, -0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1761476f, 0.1848103f) },
        new() { _pos = new Vector3D<float>(-0.09875244f, -0.4785693f, -0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1252045f, 0.1421715f) },
        new() { _pos = new Vector3D<float>(-0.2485034f, -0.3535864f, 0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.2834f) },
        new() { _pos = new Vector3D<float>(-0.1748779f, -0.3852368f, 0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8416463f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(-0.0902124f, -0.4055811f, 0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.798847f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(-0.2637085f, -0.3852368f, 0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9052075f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(-0.1839429f, -0.4252783f, 0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.2096534f) },
        new() { _pos = new Vector3D<float>(-0.09450744f, -0.4502636f, 0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8212647f, 0.1818529f) },
        new() { _pos = new Vector3D<float>(-0.2754956f, -0.4055811f, 0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9480067f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(-0.1919995f, -0.4502636f, 0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.925589f, 0.1859203f) },
        new() { _pos = new Vector3D<float>(-0.09875244f, -0.4785693f, 0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8819596f, 0.1395283f) },
        new() { _pos = new Vector3D<float>(0.2485034f, -0.3535864f, 0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.2834f) },
        new() { _pos = new Vector3D<float>(0.2637085f, -0.3852368f, 0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5918325f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(0.2754956f, -0.4055811f, 0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5490332f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(0.1748779f, -0.3852368f, 0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6553935f, 0.2507943f) },
        new() { _pos = new Vector3D<float>(0.1839429f, -0.4252783f, 0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.2142188f) },
        new() { _pos = new Vector3D<float>(0.1919995f, -0.4502636f, 0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.571451f, 0.1807698f) },
        new() { _pos = new Vector3D<float>(0.0902124f, -0.4055811f, 0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.698193f, 0.2378804f) },
        new() { _pos = new Vector3D<float>(0.09450744f, -0.4502636f, 0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6757752f, 0.1830822f) },
        new() { _pos = new Vector3D<float>(0.09875244f, -0.4785693f, 0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6150803f, 0.1321084f) },
        new() { _pos = new Vector3D<float>(0.2492358f, 0.3538598f, -0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.7253528f) },
        new() { _pos = new Vector3D<float>(0.2646435f, 0.3856372f, -0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4055798f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(0.2764746f, 0.4060791f, -0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4483792f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(0.1754565f, 0.3856372f, -0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3420188f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(0.1845666f, 0.4257813f, -0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.7708623f) },
        new() { _pos = new Vector3D<float>(0.1926355f, 0.4508106f, -0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4259613f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(0.09051941f, 0.4060791f, -0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2992194f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(0.09482116f, 0.4508106f, -0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3216372f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(0.09908752f, 0.4791992f, -0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.8706206f) },
        new() { _pos = new Vector3D<float>(0.2492358f, 0.3538598f, 0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.7253528f) },
        new() { _pos = new Vector3D<float>(0.1754565f, 0.3856372f, 0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6553935f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(0.09051941f, 0.4060791f, 0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.698193f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(0.2646435f, 0.3856372f, 0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5918325f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(0.1845666f, 0.4257813f, 0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.7708623f) },
        new() { _pos = new Vector3D<float>(0.09482116f, 0.4508106f, 0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6757752f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(0.2764746f, 0.4060791f, 0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5490332f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(0.1926355f, 0.4508106f, 0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5714509f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(0.09908752f, 0.4791992f, 0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.8706206f) },
        new() { _pos = new Vector3D<float>(-0.2492358f, 0.3538598f, 0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.7253528f) },
        new() { _pos = new Vector3D<float>(-0.2646435f, 0.3856372f, 0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9052075f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(-0.2764746f, 0.4060791f, 0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9480069f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(-0.1754565f, 0.3856372f, 0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8416463f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(-0.1845666f, 0.4257813f, 0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.7708623f) },
        new() { _pos = new Vector3D<float>(-0.1926355f, 0.4508106f, 0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.925589f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(-0.09051941f, 0.4060791f, 0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.798847f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(-0.09482116f, 0.4508106f, 0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8212647f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(-0.09908752f, 0.4791992f, 0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.8766438f) },
        new() { _pos = new Vector3D<float>(-0.2492358f, 0.3538598f, -0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.7253528f) },
        new() { _pos = new Vector3D<float>(-0.1754565f, 0.3856372f, -0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.155766f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(-0.09051879f, 0.4060791f, -0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1985654f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(-0.2646435f, 0.3856372f, -0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09220496f, 0.7454442f) },
        new() { _pos = new Vector3D<float>(-0.1845666f, 0.4257813f, -0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.7708623f) },
        new() { _pos = new Vector3D<float>(-0.09482116f, 0.4508106f, -0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1761476f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(-0.2764746f, 0.4060791f, -0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.04940557f, 0.7583581f) },
        new() { _pos = new Vector3D<float>(-0.1926355f, 0.4508106f, -0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.07182345f, 0.8105438f) },
        new() { _pos = new Vector3D<float>(-0.09908752f, 0.4791992f, -0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.8706206f) },
        new() { _pos = new Vector3D<float>(0.3535864f, -0.2485034f, -0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.401279f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(0.3852368f, -0.2637085f, -0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4309353f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(0.4055811f, -0.2754956f, -0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4638988f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(0.3852368f, -0.1748791f, -0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4032414f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(0.4252783f, -0.1839429f, -0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4337836f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(0.4502636f, -0.1919995f, -0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4658029f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(0.4055811f, -0.0902124f, -0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4038135f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(0.4502636f, -0.09450744f, -0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4346031f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(0.4785693f, -0.09875244f, -0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4663434f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(0.3535864f, -0.2485034f, 0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5961333f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(0.3852368f, -0.1748791f, 0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5941709f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(0.4055811f, -0.0902124f, 0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5935988f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(0.3852368f, -0.2637085f, 0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.566477f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(0.4252783f, -0.1839429f, 0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5636287f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(0.4502636f, -0.09450744f, 0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5628092f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(0.4055811f, -0.2754956f, 0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5335135f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(0.4502636f, -0.1919995f, 0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5316095f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(0.4785693f, -0.09875244f, 0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.531069f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(0.354751f, 0.2485034f, 0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5961334f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(0.3865088f, 0.2637475f, 0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5664771f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(0.4069678f, 0.2755518f, 0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5335135f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(0.3865088f, 0.1746521f, 0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5941709f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(0.4265039f, 0.1837427f, 0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5636287f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(0.4515527f, 0.191842f, 0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5316095f, 0.6227795f) },
        new() { _pos = new Vector3D<float>(0.4069678f, 0.08967713f, 0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5935988f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(0.4515527f, 0.09398254f, 0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5628092f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(0.479956f, 0.09823791f, 0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.531069f, 0.563587f) },
        new() { _pos = new Vector3D<float>(0.354751f, 0.2485034f, -0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4012789f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(0.3865088f, 0.1746521f, -0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4032414f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(0.4069678f, 0.08967713f, -0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4038135f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(0.3865088f, 0.2637475f, -0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4309353f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(0.4265039f, 0.1837427f, -0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4337836f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(0.4515527f, 0.09398254f, -0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4346031f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(0.4069678f, 0.2755518f, -0.09051879f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4638988f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(0.4515527f, 0.191842f, -0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4658029f, 0.6227795f) },
        new() { _pos = new Vector3D<float>(0.479956f, 0.09823791f, -0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4663434f, 0.563587f) },
        new() { _pos = new Vector3D<float>(0.2485034f, -0.2485034f, 0.3535864f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6510928f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(0.1748791f, -0.2637085f, 0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6807491f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(0.09021179f, -0.2754956f, 0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7137126f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(0.2637085f, -0.1748791f, 0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6530552f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(0.1839429f, -0.1839429f, 0.4252783f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6835974f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(0.09450744f, -0.1919995f, 0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7156168f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(0.2754956f, -0.0902124f, 0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6536273f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(0.1919995f, -0.09450744f, 0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.684417f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(0.09875244f, -0.09875244f, 0.4785693f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7161572f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(-0.2485034f, -0.2485034f, 0.3535864f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8459471f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(-0.2637085f, -0.1748779f, 0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8439847f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(-0.2754956f, -0.0902124f, 0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8434125f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(-0.1748779f, -0.2637085f, 0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8162909f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(-0.1839429f, -0.1839429f, 0.4252783f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8134426f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(-0.1919995f, -0.09450744f, 0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8126229f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(-0.09021179f, -0.2754956f, 0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7833273f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(-0.09450744f, -0.1919995f, 0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7814232f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(-0.09875244f, -0.09875244f, 0.4785693f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7808827f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(-0.2492358f, 0.2485034f, 0.354751f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8459472f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(-0.1754565f, 0.2637475f, 0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8162909f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(-0.09051941f, 0.2755518f, 0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7833273f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(-0.2646435f, 0.1746533f, 0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8439847f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(-0.1845666f, 0.1837427f, 0.4265039f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8134426f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(-0.09482116f, 0.191842f, 0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7814232f, 0.6227797f) },
        new() { _pos = new Vector3D<float>(-0.2764746f, 0.08967713f, 0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8434125f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(-0.1926355f, 0.09398254f, 0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8126229f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(-0.09908752f, 0.09823791f, 0.479956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7808827f, 0.563587f) },
        new() { _pos = new Vector3D<float>(0.2492358f, 0.2485034f, 0.354751f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6510928f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(0.2646435f, 0.1746533f, 0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6530552f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(0.2764746f, 0.08967713f, 0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6536273f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(0.1754565f, 0.2637475f, 0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6807491f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(0.1845666f, 0.1837427f, 0.4265039f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6835974f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(0.1926355f, 0.09398254f, 0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.684417f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(0.09051941f, 0.2755518f, 0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7137126f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(0.09482116f, 0.191842f, 0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7156168f, 0.6227797f) },
        new() { _pos = new Vector3D<float>(0.09908752f, 0.09823791f, 0.479956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7161572f, 0.563587f) },
        new() { _pos = new Vector3D<float>(-0.3535864f, -0.2485034f, 0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9009064f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(-0.3852368f, -0.2637085f, 0.1748791f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9305627f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(-0.4055811f, -0.2754956f, 0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9635264f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(-0.3852368f, -0.1748779f, 0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.902869f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(-0.4252783f, -0.1839429f, 0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9334112f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(-0.4502636f, -0.1919995f, 0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9654305f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(-0.4055811f, -0.0902124f, 0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9034411f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(-0.4502636f, -0.09450744f, 0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9342307f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(-0.4785693f, -0.09875244f, 0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.965971f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(-0.3535864f, -0.2485034f, -0.2485034f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09650582f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(-0.3852368f, -0.1748779f, -0.2637085f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.0945434f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(-0.4055811f, -0.0902124f, -0.2754956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09397122f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(-0.3852368f, -0.2637085f, -0.1748779f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06684953f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(-0.4252783f, -0.1839429f, -0.1839429f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06400123f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(-0.4502636f, -0.09450744f, -0.1919995f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06318161f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(-0.4055811f, -0.2754956f, -0.0902124f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03388602f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(-0.4502636f, -0.1919995f, -0.09450744f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03198189f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(-0.4785693f, -0.09875244f, -0.09875244f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03144142f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(-0.354751f, 0.2485034f, -0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09650585f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(-0.3865088f, 0.2637475f, -0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06684953f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(-0.4069678f, 0.2755518f, -0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03388602f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(-0.3865088f, 0.1746533f, -0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.0945434f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(-0.4265039f, 0.1837427f, -0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06400123f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(-0.4515527f, 0.191842f, -0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03198189f, 0.6227797f) },
        new() { _pos = new Vector3D<float>(-0.4069678f, 0.08967713f, -0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.09397122f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(-0.4515527f, 0.09398254f, -0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.06318161f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(-0.479956f, 0.09823791f, -0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03144142f, 0.563587f) },
        new() { _pos = new Vector3D<float>(-0.354751f, 0.2485034f, 0.2492358f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9009064f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(-0.3865088f, 0.1746533f, 0.2646435f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.902869f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(-0.4069678f, 0.08967713f, 0.2764746f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9034411f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(-0.3865088f, 0.2637475f, 0.1754565f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9305627f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(-0.4265039f, 0.1837427f, 0.1845666f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9334112f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(-0.4515527f, 0.09398254f, 0.1926355f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9342307f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(-0.4069678f, 0.2755518f, 0.09051941f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9635264f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(-0.4515527f, 0.191842f, 0.09482116f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9654305f, 0.6227797f) },
        new() { _pos = new Vector3D<float>(-0.479956f, 0.09823791f, 0.09908752f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.965971f, 0.563587f) },
        new() { _pos = new Vector3D<float>(-0.2485034f, -0.2485034f, -0.3535864f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1514652f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(-0.1748791f, -0.2637085f, -0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1811215f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(-0.0902124f, -0.2754956f, -0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.214085f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(-0.2637085f, -0.1748779f, -0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1534276f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(-0.1839429f, -0.1839429f, -0.4252783f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1839698f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(-0.09450744f, -0.1919995f, -0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2159891f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(-0.2754956f, -0.0902124f, -0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1539998f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(-0.1919995f, -0.09450744f, -0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1847894f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(-0.09875244f, -0.09875244f, -0.4785693f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2165296f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(0.2485034f, -0.2485034f, -0.3535864f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3463196f, 0.3431527f) },
        new() { _pos = new Vector3D<float>(0.2637085f, -0.1748791f, -0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3443572f, 0.3898893f) },
        new() { _pos = new Vector3D<float>(0.2754956f, -0.0902124f, -0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.343785f, 0.4436346f) },
        new() { _pos = new Vector3D<float>(0.1748779f, -0.2637085f, -0.3852368f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3166633f, 0.3335016f) },
        new() { _pos = new Vector3D<float>(0.1839429f, -0.1839429f, -0.4252783f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.313815f, 0.3841356f) },
        new() { _pos = new Vector3D<float>(0.1919995f, -0.09450744f, -0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3129954f, 0.4409079f) },
        new() { _pos = new Vector3D<float>(0.0902124f, -0.2754956f, -0.4055811f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2836998f, 0.3260186f) },
        new() { _pos = new Vector3D<float>(0.09450744f, -0.1919995f, -0.4502636f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2817956f, 0.3790208f) },
        new() { _pos = new Vector3D<float>(0.09875244f, -0.09875244f, -0.4785693f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2812552f, 0.4382135f) },
        new() { _pos = new Vector3D<float>(0.2492358f, 0.2485034f, -0.354751f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3463196f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(0.1754565f, 0.2637475f, -0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3166633f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(0.09051879f, 0.2755518f, -0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2836998f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(0.2646435f, 0.1746533f, -0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3443572f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(0.1845666f, 0.1837427f, -0.4265039f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.313815f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(0.09482116f, 0.191842f, -0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2817956f, 0.6227797f) },
        new() { _pos = new Vector3D<float>(0.2764746f, 0.08967713f, -0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.343785f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(0.1926355f, 0.09398254f, -0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3129954f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(0.09908752f, 0.09823791f, -0.479956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2812552f, 0.563587f) },
        new() { _pos = new Vector3D<float>(-0.2492358f, 0.2485034f, -0.354751f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1514652f, 0.6586478f) },
        new() { _pos = new Vector3D<float>(-0.2646435f, 0.1746533f, -0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1534276f, 0.6119112f) },
        new() { _pos = new Vector3D<float>(-0.2764746f, 0.08967713f, -0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1539998f, 0.5581658f) },
        new() { _pos = new Vector3D<float>(-0.1754565f, 0.2637475f, -0.3865088f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1811215f, 0.6682988f) },
        new() { _pos = new Vector3D<float>(-0.1845666f, 0.1837427f, -0.4265039f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1839698f, 0.6176648f) },
        new() { _pos = new Vector3D<float>(-0.1926355f, 0.09398254f, -0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1847894f, 0.5608926f) },
        new() { _pos = new Vector3D<float>(-0.09051879f, 0.2755518f, -0.4069678f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.214085f, 0.6757817f) },
        new() { _pos = new Vector3D<float>(-0.09482116f, 0.191842f, -0.4515527f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2159891f, 0.6227797f) },
        new() { _pos = new Vector3D<float>(-0.09908752f, 0.09823791f, -0.479956f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2165296f, 0.563587f) },
        new() { _pos = new Vector3D<float>(0.1007947f, -0.4882324f, -9.536743E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.09756867f) },
        new() { _pos = new Vector3D<float>(0, -0.5f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3734581f, 0.02175571f) },
        new() { _pos = new Vector3D<float>(0, -0.5f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.119418f, 0.02175571f) },
        new() { _pos = new Vector3D<float>(-0.3525147f, -0.3525147f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(-0.2801587f, -0.4123633f, 1.907349E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.233575f) },
        new() { _pos = new Vector3D<float>(-0.1956055f, -0.4585547f, -3.814694E-10f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.1733436f) },
        new() { _pos = new Vector3D<float>(-0.1007953f, -0.4882324f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.1035097f) },
        new() { _pos = new Vector3D<float>(0, -0.5f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8739171f, 0.02038012f) },
        new() { _pos = new Vector3D<float>(0, -0.5f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8739171f, 0.02038012f) },
        new() { _pos = new Vector3D<float>(0, -0.5f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6224174f, 0.01762898f) },
        new() { _pos = new Vector3D<float>(0.1007947f, -0.4882324f, -9.536743E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.09756867f) },
        new() { _pos = new Vector3D<float>(4.764297E-09f, 0.5f, -4.768371E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3847829f, 0.9717647f) },
        new() { _pos = new Vector3D<float>(4.764297E-09f, 0.5f, -4.768371E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6197699f, 0.9758912f) },
        new() { _pos = new Vector3D<float>(4.764297E-09f, 0.5f, -4.768371E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8750802f, 0.9731402f) },
        new() { _pos = new Vector3D<float>(4.764297E-09f, 0.5f, -4.768371E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8750802f, 0.9731402f) },
        new() { _pos = new Vector3D<float>(-0.3536914f, 0.3527881f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(-0.2811499f, 0.4128613f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.7626636f) },
        new() { _pos = new Vector3D<float>(-0.1962756f, 0.4591016f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.8162426f) },
        new() { _pos = new Vector3D<float>(-0.1011359f, 0.4888574f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.9020402f) },
        new() { _pos = new Vector3D<float>(4.764297E-09f, 0.5f, -4.768371E-09f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1294726f, 0.9717647f) },
        new() { _pos = new Vector3D<float>(0.2878271f, -0.2878271f, -0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.3089014f, -0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3935962f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.3313476f, -0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4232635f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.3469702f, -0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4591776f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.3525147f, -0.3525147f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.3089014f, 0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6038162f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.2878271f, -0.2878271f, 0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.3313476f, 0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5741488f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.3469702f, 0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5382347f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.2888354f, 0.2879004f, 0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.683609f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.3090894f, 0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6038162f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.3315698f, 0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5741488f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.3472266f, 0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.5382347f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(0.3536914f, 0.3527881f, -1.144409E-07f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4987062f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.3090894f, -0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3935962f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.2888354f, 0.2879004f, -0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.683609f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.3315698f, -0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4232635f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.3472266f, -0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.4591776f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(0.2878271f, -0.2878271f, 0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.2401929f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.348428f) },
        new() { _pos = new Vector3D<float>(0.2401929f, -0.3089014f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6434099f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.1701416f, -0.3313476f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6730773f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.0880603f, -0.3469702f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7089914f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(5.722046E-08f, -0.3525147f, 0.3525147f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.1701416f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.0880603f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(0.3535523f, -3.128662E-11f, 0.3535523f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.2401929f, -0.3089014f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8536299f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(-0.2878271f, -0.2878271f, 0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(-0.1701416f, -0.3313476f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8239627f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.0880603f, -0.3469702f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7880484f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.2888354f, 0.2879004f, 0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.683609f) },
        new() { _pos = new Vector3D<float>(-0.2409058f, 0.3090894f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8536299f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.1707129f, 0.3315698f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8239627f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.08835449f, 0.3472266f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7880484f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(5.722046E-08f, 0.3527881f, 0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.74852f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(0.2409058f, 0.3090894f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6434099f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.2401733f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(0.2888354f, 0.2879004f, 0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.683609f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.1698926f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.08750671f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6236131f, 0.5568f) },
        new() { _pos = new Vector3D<float>(0.1707129f, 0.3315698f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.6730773f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.08835449f, 0.3472266f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.7089914f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.2878271f, -0.2878271f, 0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.2401929f, 0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.348428f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.3089014f, 0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8932236f, 0.3048139f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.3313476f, 0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.922891f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.3469702f, 0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9588053f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.3525147f, -0.3525147f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.1701416f, 0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.0880603f, 0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(-0.3535523f, 3.128662E-11f, 0.3535523f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.3089014f, -0.2401929f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1041886f, 0.3048139f) },
        new() { _pos = new Vector3D<float>(-0.2878271f, -0.2878271f, -0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.3313476f, -0.1701416f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.07452133f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.3469702f, -0.0880603f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03860712f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-0.3525147f, -0.3525147f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(-0.4123633f, -0.2801587f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.3230592f) },
        new() { _pos = new Vector3D<float>(-0.4585547f, -0.1956055f, 3.852852E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.3767318f) },
        new() { _pos = new Vector3D<float>(-0.4882324f, -0.1007947f, 3.814697E-08f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.4369167f) },
        new() { _pos = new Vector3D<float>(-0.5f, 4.764297E-09f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(-0.2888354f, 0.2879004f, -0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6836091f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.3090894f, -0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1041886f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.3315698f, -0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.07452133f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.3472266f, -0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.03860712f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.3536914f, 0.3527881f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(-0.0009213686f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.3090894f, 0.2409058f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8932238f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.2401733f, 0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(-0.2888354f, 0.2879004f, 0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.683609f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.1698926f, 0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.08750671f, 0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.8734268f, 0.5568f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.3315698f, 0.1707129f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.922891f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.3472266f, 0.08835449f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9588053f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-0.3536914f, 0.3527881f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(-0.4137475f, 0.2802221f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.6787412f) },
        new() { _pos = new Vector3D<float>(-0.4598437f, 0.195459f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.6250687f) },
        new() { _pos = new Vector3D<float>(-0.4896582f, 0.1002972f, 0), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.9983337f, 0.5648838f) },
        new() { _pos = new Vector3D<float>(-0.2878271f, -0.2878271f, -0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(-0.3089014f, -0.2401929f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.348428f) },
        new() { _pos = new Vector3D<float>(-0.2401929f, -0.3089014f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1437824f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(-0.1701416f, -0.3313476f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1734497f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(-0.0880603f, -0.3469702f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2093638f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(-5.340553E-08f, -0.3525147f, -0.3525147f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.2771286f) },
        new() { _pos = new Vector3D<float>(-0.3313476f, -0.1701416f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(-0.3469702f, -0.0880603f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(-0.3536914f, -0.0008836116f, -0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.2401929f, -0.3089014f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3540024f, 0.3048138f) },
        new() { _pos = new Vector3D<float>(0.3089014f, -0.2401929f, -0.3089014f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.348428f) },
        new() { _pos = new Vector3D<float>(0.2878271f, -0.2878271f, -0.2878271f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.3181914f) },
        new() { _pos = new Vector3D<float>(0.3313476f, -0.1701416f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.3928966f) },
        new() { _pos = new Vector3D<float>(0.3469702f, -0.0880603f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.4450004f) },
        new() { _pos = new Vector3D<float>(0.3536914f, -0.0008836466f, -0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.5009002f) },
        new() { _pos = new Vector3D<float>(0.1701416f, -0.3313476f, -0.3313476f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3243351f, 0.2905639f) },
        new() { _pos = new Vector3D<float>(0.0880603f, -0.3469702f, -0.3469702f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2884209f, 0.2806473f) },
        new() { _pos = new Vector3D<float>(0.2888354f, 0.2879004f, -0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.683609f) },
        new() { _pos = new Vector3D<float>(0.3099512f, 0.2401733f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(0.2409058f, 0.3090894f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3540024f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(0.1707129f, 0.3315698f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3243351f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(0.08835449f, 0.3472266f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2884209f, 0.7211531f) },
        new() { _pos = new Vector3D<float>(-1.573563E-07f, 0.3527881f, -0.3536914f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2488924f, 0.7246718f) },
        new() { _pos = new Vector3D<float>(0.3324731f, 0.1698926f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(0.3481274f, 0.08750671f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.3737993f, 0.5568f) },
        new() { _pos = new Vector3D<float>(-0.2409058f, 0.3090894f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1437824f, 0.6969866f) },
        new() { _pos = new Vector3D<float>(-0.3099512f, 0.2401733f, -0.3099512f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6533725f) },
        new() { _pos = new Vector3D<float>(-0.2888354f, 0.2879004f, -0.2888354f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6836091f) },
        new() { _pos = new Vector3D<float>(-0.3324731f, 0.1698926f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.6089039f) },
        new() { _pos = new Vector3D<float>(-0.3481274f, 0.08750671f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1239855f, 0.5568f) },
        new() { _pos = new Vector3D<float>(-0.1707129f, 0.3315698f, -0.3324731f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.1734497f, 0.7112365f) },
        new() { _pos = new Vector3D<float>(-0.08835449f, 0.3472266f, -0.3481274f), _color = new Vector3D<float>(1, 1, 1), _texCoord = new Vector2D<float>(0.2093638f, 0.7211531f) },
    };

    readonly ushort[] _indices =
    {
        177, 386, 178, 174, 177, 178, 173, 177, 174, 173, 176, 177, 176, 108, 177, 177, 108, 109, 178, 109, 8, 205, 109, 108, 174, 178, 175, 175, 178, 100, 178, 387, 100, 170, 173, 174, 171, 174, 175, 170, 174, 171, 48, 176, 173, 47,
        48, 173, 47, 173, 170, 0, 47, 170, 0, 170, 26, 26, 170, 171, 48, 49, 176, 26, 171, 27, 49, 107, 176, 49, 17, 107, 176, 107, 108, 27, 171, 172, 171, 175, 172, 27, 172, 28, 199, 107, 17, 199, 17, 46, 202, 108, 107, 202, 107, 199,
        205, 108, 202, 172, 175, 99, 175, 100, 99, 28, 172, 98, 172, 99, 98, 28, 98, 14, 198, 199, 46, 198, 46, 45, 201, 202, 199, 201, 199, 198, 14, 98, 185, 14, 185, 31, 98, 99, 186, 98, 186, 185, 99, 100, 187, 99, 187, 186, 197, 198,
        45, 197, 45, 44, 31, 185, 182, 31, 182, 30, 100, 103, 187, 100, 388, 103, 187, 103, 102, 187, 102, 184, 186, 187, 184, 184, 102, 101, 185, 186, 183, 186, 184, 183, 185, 183, 182, 184, 101, 181, 183, 184, 181, 181, 101, 15, 181,
        15, 34, 183, 181, 180, 180, 181, 34, 182, 183, 180, 180, 34, 33, 182, 180, 179, 179, 180, 33, 30, 182, 179, 179, 33, 32, 30, 179, 29, 29, 179, 32, 29, 32, 2, 106, 395, 109, 196, 394, 106, 193, 196, 106, 193, 106, 105, 105, 106,
        205, 106, 396, 205, 105, 205, 204, 204, 205, 202, 204, 202, 201, 104, 105, 204, 203, 204, 201, 104, 204, 203, 190, 193, 105, 190, 105, 104, 203, 201, 200, 200, 201, 198, 200, 198, 197, 16, 104, 203, 40, 104, 16, 40, 190, 104,
        16, 203, 43, 43, 203, 200, 43, 200, 42, 42, 200, 197, 42, 197, 41, 41, 197, 44, 41, 44, 1, 39, 190, 40, 39, 189, 190, 38, 189, 39, 189, 193, 190, 38, 188, 189, 3, 188, 38, 3, 35, 188, 189, 192, 193, 188, 192, 189, 192, 196, 193,
        35, 191, 188, 188, 191, 192, 35, 36, 191, 192, 195, 196, 191, 195, 192, 36, 194, 191, 191, 194, 195, 36, 37, 194, 195, 392, 196, 196, 392, 393, 195, 391, 392, 194, 391, 195, 194, 390, 391, 37, 390, 194, 37, 389, 390, 214, 121,
        9, 213, 121, 214, 210, 213, 214, 210, 214, 211, 211, 214, 112, 214, 397, 112, 213, 120, 121, 209, 213, 210, 212, 120, 213, 209, 212, 213, 207, 210, 211, 206, 209, 210, 206, 210, 207, 72, 212, 209, 71, 72, 209, 71, 209, 206, 4,
        71, 206, 4, 206, 50, 50, 206, 207, 72, 73, 212, 50, 207, 51, 73, 119, 212, 212, 119, 120, 73, 21, 119, 51, 207, 208, 207, 211, 208, 51, 208, 52, 235, 119, 21, 235, 21, 70, 238, 120, 119, 238, 119, 235, 208, 211, 111, 211, 112,
        111, 52, 208, 110, 208, 111, 110, 52, 110, 18, 234, 235, 70, 234, 70, 69, 18, 110, 221, 18, 221, 55, 110, 111, 222, 110, 222, 221, 111, 112, 223, 111, 223, 222, 55, 221, 218, 55, 218, 54, 221, 222, 219, 221, 219, 218, 54, 218,
        215, 54, 215, 53, 53, 215, 56, 53, 56, 5, 215, 57, 56, 218, 216, 215, 215, 216, 57, 218, 219, 216, 216, 58, 57, 216, 217, 58, 219, 217, 216, 217, 19, 58, 222, 220, 219, 219, 220, 217, 222, 223, 220, 217, 113, 19, 220, 113, 217,
        61, 19, 113, 223, 114, 220, 220, 114, 113, 61, 113, 230, 230, 113, 114, 60, 61, 230, 223, 115, 114, 112, 115, 223, 112, 398, 115, 60, 230, 227, 59, 60, 227, 230, 114, 231, 231, 114, 115, 227, 230, 231, 59, 227, 224, 7, 59, 224,
        7, 224, 62, 231, 115, 232, 232, 115, 399, 227, 231, 228, 224, 227, 228, 228, 231, 232, 62, 224, 225, 224, 228, 225, 62, 225, 63, 228, 232, 229, 225, 228, 229, 63, 225, 226, 225, 229, 226, 63, 226, 64, 229, 232, 118, 232, 400,
        118, 229, 118, 117, 226, 229, 117, 226, 117, 116, 64, 226, 116, 64, 116, 20, 65, 68, 6, 65, 233, 68, 233, 69, 68, 66, 233, 65, 233, 234, 69, 66, 236, 233, 236, 234, 233, 67, 236, 66, 236, 237, 234, 237, 235, 234, 237, 238, 235,
        67, 239, 236, 239, 237, 236, 401, 239, 67, 401, 402, 239, 239, 240, 237, 402, 240, 239, 240, 238, 237, 402, 403, 240, 240, 241, 238, 403, 241, 240, 241, 120, 238, 403, 404, 241, 241, 121, 120, 404, 121, 241, 404, 405, 121, 411,
        74, 412, 411, 251, 74, 413, 251, 411, 251, 75, 74, 413, 254, 251, 414, 254, 413, 251, 252, 75, 254, 252, 251, 252, 76, 75, 414, 257, 254, 410, 257, 414, 252, 253, 76, 253, 22, 76, 254, 255, 252, 257, 255, 254, 255, 253, 252,
        410, 122, 257, 409, 122, 410, 253, 125, 22, 79, 22, 125, 255, 256, 253, 256, 125, 253, 257, 258, 255, 122, 258, 257, 258, 256, 255, 409, 244, 122, 408, 244, 409, 79, 125, 266, 78, 79, 266, 122, 123, 258, 244, 123, 122, 256, 126,
        125, 266, 125, 126, 258, 259, 256, 123, 259, 258, 259, 126, 256, 408, 243, 244, 407, 243, 408, 78, 266, 263, 77, 78, 263, 244, 247, 123, 243, 247, 244, 123, 124, 259, 247, 124, 123, 259, 127, 126, 124, 127, 259, 266, 126, 267,
        263, 266, 267, 267, 126, 127, 407, 242, 243, 406, 242, 407, 406, 83, 242, 243, 246, 247, 242, 246, 243, 83, 245, 242, 242, 245, 246, 83, 84, 245, 247, 250, 124, 246, 250, 247, 124, 10, 127, 250, 10, 124, 267, 127, 268, 268, 127,
        10, 84, 248, 245, 84, 85, 248, 245, 249, 246, 246, 249, 250, 245, 248, 249, 250, 133, 10, 249, 133, 250, 85, 131, 248, 85, 23, 131, 248, 132, 249, 249, 132, 133, 248, 131, 132, 130, 10, 133, 268, 10, 130, 264, 267, 268, 263,
        267, 264, 271, 131, 23, 271, 23, 82, 274, 132, 131, 274, 131, 271, 277, 133, 132, 130, 133, 277, 277, 132, 274, 265, 268, 130, 264, 268, 265, 260, 263, 264, 77, 263, 260, 415, 77, 260, 415, 260, 416, 260, 264, 261, 416, 260,
        261, 261, 264, 265, 416, 261, 417, 417, 261, 262, 261, 265, 262, 417, 262, 418, 265, 130, 129, 262, 265, 129, 129, 130, 277, 418, 262, 128, 262, 129, 128, 418, 128, 419, 129, 277, 276, 128, 129, 276, 276, 277, 274, 419, 128,
        275, 128, 276, 275, 419, 275, 423, 276, 274, 273, 275, 276, 273, 273, 274, 271, 423, 275, 272, 275, 273, 272, 423, 272, 422, 273, 271, 270, 272, 273, 270, 270, 271, 82, 270, 82, 81, 422, 272, 269, 272, 270, 269, 269, 270, 81,
        422, 269, 420, 269, 81, 80, 420, 269, 80, 420, 80, 421, 433, 86, 434, 433, 287, 86, 435, 287, 433, 287, 87, 86, 435, 290, 287, 436, 290, 435, 287, 288, 87, 290, 288, 287, 288, 88, 87, 436, 293, 290, 429, 293, 436, 288, 289, 88,
        289, 24, 88, 290, 291, 288, 293, 291, 290, 291, 289, 288, 429, 134, 293, 428, 134, 429, 289, 137, 24, 91, 24, 137, 291, 292, 289, 292, 137, 289, 293, 294, 291, 134, 294, 293, 294, 292, 291, 428, 280, 134, 427, 280, 428, 91, 137,
        302, 90, 91, 302, 134, 135, 294, 280, 135, 134, 292, 138, 137, 302, 137, 138, 294, 295, 292, 135, 295, 294, 295, 138, 292, 427, 279, 280, 426, 279, 427, 90, 302, 299, 89, 90, 299, 280, 283, 135, 279, 283, 280, 135, 136, 295,
        283, 136, 135, 295, 139, 138, 136, 139, 295, 302, 138, 303, 299, 302, 303, 303, 138, 139, 426, 278, 279, 424, 278, 426, 424, 425, 278, 279, 282, 283, 278, 282, 279, 425, 281, 278, 278, 281, 282, 425, 430, 281, 283, 286, 136,
        282, 286, 283, 136, 11, 139, 286, 11, 136, 303, 139, 304, 304, 139, 11, 430, 284, 281, 430, 431, 284, 281, 285, 282, 282, 285, 286, 281, 284, 285, 286, 145, 11, 285, 145, 286, 431, 143, 284, 431, 432, 143, 284, 144, 285, 285,
        144, 145, 284, 143, 144, 142, 11, 145, 304, 11, 142, 300, 303, 304, 299, 303, 300, 307, 143, 432, 307, 432, 446, 310, 144, 143, 310, 143, 307, 313, 145, 144, 142, 145, 313, 313, 144, 310, 301, 304, 142, 300, 304, 301, 296, 299,
        300, 89, 299, 296, 437, 89, 296, 437, 296, 438, 296, 300, 297, 438, 296, 297, 297, 300, 301, 438, 297, 439, 439, 297, 298, 297, 301, 298, 439, 298, 440, 301, 142, 141, 298, 301, 141, 141, 142, 313, 440, 298, 140, 298, 141, 140,
        440, 140, 441, 141, 313, 312, 140, 141, 312, 312, 313, 310, 441, 140, 311, 140, 312, 311, 441, 311, 448, 312, 310, 309, 311, 312, 309, 309, 310, 307, 448, 311, 308, 311, 309, 308, 448, 308, 447, 309, 307, 306, 308, 309, 306,
        306, 307, 446, 306, 446, 445, 447, 308, 305, 308, 306, 305, 305, 306, 445, 447, 305, 442, 305, 445, 443, 442, 305, 443, 442, 443, 444, 453, 146, 454, 453, 316, 146, 452, 316, 453, 316, 147, 146, 452, 315, 316, 451, 315, 452,
        316, 319, 147, 315, 319, 316, 319, 148, 147, 451, 314, 315, 449, 314, 451, 449, 450, 314, 315, 318, 319, 314, 318, 315, 319, 322, 148, 318, 322, 319, 322, 12, 148, 450, 317, 314, 314, 317, 318, 450, 455, 317, 318, 321, 322, 317,
        321, 318, 322, 157, 12, 321, 157, 322, 482, 12, 157, 455, 320, 317, 317, 320, 321, 455, 456, 320, 482, 157, 349, 481, 482, 349, 321, 156, 157, 320, 156, 321, 349, 157, 156, 456, 155, 320, 320, 155, 156, 456, 457, 155, 481, 349,
        348, 480, 481, 348, 349, 156, 346, 346, 156, 155, 348, 349, 346, 343, 155, 457, 346, 155, 343, 343, 457, 476, 480, 348, 347, 479, 480, 347, 479, 347, 478, 348, 346, 345, 345, 346, 343, 347, 348, 345, 342, 343, 476, 345, 343,
        342, 342, 476, 475, 478, 347, 344, 347, 345, 344, 344, 345, 342, 478, 344, 477, 341, 342, 475, 344, 342, 341, 477, 344, 341, 341, 475, 473, 477, 341, 472, 472, 341, 473, 472, 473, 474, 458, 92, 459, 458, 323, 92, 460, 323, 458,
        323, 93, 92, 460, 326, 323, 461, 326, 460, 323, 324, 93, 326, 324, 323, 324, 94, 93, 461, 329, 326, 462, 329, 461, 462, 463, 329, 324, 325, 94, 325, 25, 94, 326, 327, 324, 329, 327, 326, 327, 325, 324, 463, 330, 329, 329, 330,
        327, 463, 464, 330, 325, 149, 25, 97, 25, 149, 327, 328, 325, 330, 328, 327, 328, 149, 325, 464, 331, 330, 330, 331, 328, 464, 465, 331, 97, 149, 338, 96, 97, 338, 328, 150, 149, 331, 150, 328, 338, 149, 150, 465, 151, 331, 331,
        151, 150, 465, 466, 151, 96, 338, 335, 95, 96, 335, 338, 150, 339, 339, 150, 151, 335, 338, 339, 340, 151, 466, 339, 151, 340, 340, 466, 154, 95, 335, 332, 467, 95, 332, 467, 332, 468, 335, 339, 336, 336, 339, 340, 332, 335,
        336, 337, 340, 154, 336, 340, 337, 337, 154, 153, 468, 332, 333, 332, 336, 333, 333, 336, 337, 468, 333, 469, 334, 337, 153, 333, 337, 334, 469, 333, 334, 334, 153, 152, 469, 334, 470, 470, 334, 152, 470, 152, 471, 492, 493,
        494, 492, 359, 493, 498, 359, 492, 359, 495, 493, 498, 362, 359, 499, 362, 498, 359, 360, 495, 362, 360, 359, 360, 496, 495, 499, 365, 362, 488, 365, 499, 360, 361, 496, 361, 497, 496, 362, 363, 360, 365, 363, 362, 363, 361,
        360, 488, 158, 365, 487, 158, 488, 361, 161, 497, 507, 497, 161, 363, 364, 361, 364, 161, 361, 365, 366, 363, 158, 366, 365, 366, 364, 363, 487, 352, 158, 486, 352, 487, 507, 161, 374, 506, 507, 374, 158, 159, 366, 352, 159,
        158, 364, 162, 161, 374, 161, 162, 366, 367, 364, 159, 367, 366, 367, 162, 364, 486, 351, 352, 485, 351, 486, 506, 374, 371, 501, 506, 371, 352, 355, 159, 351, 355, 352, 159, 160, 367, 355, 160, 159, 367, 163, 162, 160, 163,
        367, 374, 162, 375, 371, 374, 375, 375, 162, 163, 485, 350, 351, 483, 350, 485, 483, 484, 350, 351, 354, 355, 350, 354, 351, 484, 353, 350, 350, 353, 354, 484, 489, 353, 355, 358, 160, 354, 358, 355, 160, 13, 163, 358, 13, 160,
        375, 163, 376, 376, 163, 13, 489, 356, 353, 489, 490, 356, 353, 357, 354, 354, 357, 358, 353, 356, 357, 358, 169, 13, 357, 169, 358, 490, 167, 356, 490, 491, 167, 356, 168, 357, 357, 168, 169, 356, 167, 168, 166, 13, 169, 376,
        13, 166, 372, 375, 376, 371, 375, 372, 379, 167, 491, 379, 491, 512, 382, 168, 167, 382, 167, 379, 385, 169, 168, 166, 169, 385, 385, 168, 382, 373, 376, 166, 372, 376, 373, 368, 371, 372, 501, 371, 368, 500, 501, 368, 500, 368,
        502, 368, 372, 369, 502, 368, 369, 369, 372, 373, 502, 369, 503, 503, 369, 370, 369, 373, 370, 503, 370, 504, 373, 166, 165, 370, 373, 165, 165, 166, 385, 504, 370, 164, 370, 165, 164, 504, 164, 505, 165, 385, 384, 164, 165,
        384, 384, 385, 382, 505, 164, 383, 164, 384, 383, 505, 383, 514, 384, 382, 381, 383, 384, 381, 381, 382, 379, 514, 383, 380, 383, 381, 380, 514, 380, 513, 381, 379, 378, 380, 381, 378, 378, 379, 512, 378, 512, 511, 513, 380,
        377, 380, 378, 377, 377, 378, 511, 513, 377, 508, 377, 511, 509, 508, 377, 509, 508, 509, 510
    };


    public Example2(Surface surface, Func<string, Stream> assetLoader)
    {
        _surface = surface;

        _playerLoop = CreatePlayerLoop();
        _surface.BeforeDraw += _playerLoop.Run;

        var pipeline = _surface.CreatePipeLine(GetVertexShader(assetLoader), GetFragmentShader(assetLoader), Vertex.GetBindingDescription(), Vertex.GetAttributeDescriptions(), GetBindings());
        HelloTexture texture;
        using (var img = GetImage(assetLoader, "Textures/grey.png"))
        {
            texture = _surface.CreateTexture(img);
        }

        _camera = new Node("camera");
        var camera = new Camera(_surface) { Orthographic = true, };
        _camera.AddComponent(camera);

        _camera.LocalPosition = new Vector3(0, 0, 5);
        
        var vb = _surface.CreateVertexBuffer(_vertices);
        var ib = _surface.CreateIndexBuffer(_indices);

        var renderer1 = new Node("renderer");
        renderer1.AddComponent(new Renderer(_surface, pipeline, vb, ib, texture));
        renderer1.LocalScale = new Vector3(0.5f);
        renderer1.LocalPosition = new Vector3(-1f, -1f, 0);

        renderer1.AddComponent(new SimpleAnimator() { Animation = CreateAnimation(CreateLinearAnimationCurve()), Mode = SimpleAnimator.PlayMode.PING_PONG });

        _renderers.Add(renderer1);

        var renderer2 = new Node("renderer");
        renderer2.AddComponent(new Renderer(_surface, pipeline, vb, ib, texture));
        renderer2.LocalScale = new Vector3(0.5f);
        renderer2.LocalPosition = new Vector3(0, -1f, 0);

        renderer2.AddComponent(new SimpleAnimator() { Animation = CreateAnimation(CreateEasingInOutAnimationCurve()), Mode = SimpleAnimator.PlayMode.PING_PONG });
        _renderers.Add(renderer2);

        var renderer3 = new Node("renderer");
        renderer3.AddComponent(new Renderer(_surface, pipeline, vb, ib, texture));
        renderer3.LocalScale = new Vector3(0.5f);
        renderer3.LocalPosition = new Vector3(1f, -1f, 0);

        renderer3.AddComponent(new SimpleAnimator() { Animation = CreateAnimation(CreateEasingInAnimationCurve()), Mode = SimpleAnimator.PlayMode.PING_PONG });
        _renderers.Add(renderer3);
    }

    static DescriptorSetLayoutBinding[] GetBindings() =>
    [
        new DescriptorSetLayoutBinding
        {
            Binding = 0,
            DescriptorCount = 1,
            DescriptorType = DescriptorType.UniformBuffer,
            PImmutableSamplers = null,
            StageFlags = ShaderStageFlags.VertexBit,
        },

        new DescriptorSetLayoutBinding
        {
            Binding = 1,
            DescriptorCount = 1,
            DescriptorType = DescriptorType.CombinedImageSampler,
            PImmutableSamplers = null,
            StageFlags = ShaderStageFlags.FragmentBit,
        }
    ];

    byte[] GetFragmentShader(Func<string, Stream> assetLoader) => GetBytes(assetLoader, "Shaders/frag.spv");
    byte[] GetVertexShader(Func<string, Stream> assetLoader) => GetBytes(assetLoader, "Shaders/vert.spv");

    Image<Rgba32> GetImage(Func<string, Stream> assetLoader, string path)
    {
        using var stream = assetLoader(path);
        return Image.Load<Rgba32>(stream);
    }

    byte[] GetBytes(Func<string, Stream> assetLoader, string path)
    {
        using var stream = assetLoader(path);
        using var memStream = new MemoryStream();
        stream.CopyTo(memStream);
        return memStream.ToArray();
    }

    static Curve CreateLinearAnimationCurve()
    {
        var animationCurve1 = new Curve();
        animationCurve1.AddKeyframe(new Keyframe(0, -2, 1, 1));
        animationCurve1.AddKeyframe(new Keyframe(2, 2, 1, 1));
        return animationCurve1;
    }

    static Curve CreateEasingInOutAnimationCurve()
    {
        var animationCurve1 = new Curve();
        animationCurve1.AddKeyframe(new Keyframe(0, -2, 0, 0));
        animationCurve1.AddKeyframe(new Keyframe(2, 2, 0, 0));
        return animationCurve1;
    }

    static Curve CreateEasingInAnimationCurve()
    {
        var animationCurve1 = new Curve();
        animationCurve1.AddKeyframe(new Keyframe(0, -2, 0, 0));
        animationCurve1.AddKeyframe(new Keyframe(2, 2, 2, 2));
        return animationCurve1;
    }

    static Clip CreateAnimation(Curve curve)
    {
        return new Clip
        {
            Tracks = new List<Track>()
            {
                new Track(new AnimationBinding("", "localPosition.y"), curve)
            },
        };
    }

    static PlayerLoop CreatePlayerLoop()
    {
        var playerLoop = new PlayerLoop();
        playerLoop.Add(new UpdateStage());
        playerLoop.Add(new AnimationStage());
        playerLoop.Add(new RenderableStage());
        return playerLoop;
    }
}
